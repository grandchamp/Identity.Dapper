using Dapper;
using Identity.Dapper.Connections;
using Identity.Dapper.Entities;
using Identity.Dapper.Models;
using Identity.Dapper.Repositories.Contracts;
using Identity.Dapper.UnitOfWork.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Identity.Dapper.Repositories
{
    public class RoleRepository<TRole, TKey, TUserRole, TRoleClaim>
        : IRoleRepository<TRole, TKey, TUserRole, TRoleClaim>
        where TRole : DapperIdentityRole<TKey, TUserRole, TRoleClaim>
        where TKey : IEquatable<TKey>
        where TUserRole : DapperIdentityUserRole<TKey>
        where TRoleClaim : DapperIdentityRoleClaim<TKey>
    {
        private readonly IConnectionProvider _connectionProvider;
        private readonly ILogger<RoleRepository<TRole, TKey, TUserRole, TRoleClaim>> _log;
        private readonly SqlConfiguration _sqlConfiguration;
        private readonly IUnitOfWork _unitOfWork;
        public RoleRepository(IConnectionProvider connProv, ILogger<RoleRepository<TRole, TKey, TUserRole, TRoleClaim>> log, SqlConfiguration sqlConf, IUnitOfWork uow)
        {
            _connectionProvider = connProv;
            _log = log;
            _sqlConfiguration = sqlConf;
            _unitOfWork = uow;
        }

        public async Task<TRole> GetById(TKey id)
        {
            try
            {
                var selectFunction = new Func<DbConnection, Task<TRole>>(async x =>
                {
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("Id", id);

                    var query = _sqlConfiguration.SelectRoleByIdQuery
                                                 .ReplaceQueryParameters(_sqlConfiguration.SchemaName,
                                                                         _sqlConfiguration.RoleTable,
                                                                         _sqlConfiguration.ParameterNotation,
                                                                         new string[] { "%ID%" },
                                                                         new string[] { "Id" });

                    return await x.QueryFirstOrDefaultAsync<TRole>(sql: query,
                                                                   param: dynamicParameters,
                                                                   transaction: _unitOfWork.Transaction);
                });

                DbConnection conn = null;
                if (_unitOfWork?.Connection == null)
                {
                    using (conn = _connectionProvider.Create())
                    {
                        await conn.OpenAsync();

                        return await selectFunction(conn);
                    }
                }
                else
                {
                    conn = _unitOfWork.CreateOrGetConnection();
                    return await selectFunction(conn);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return null;
            }
        }

        public async Task<TRole> GetByName(string roleName)
        {
            try
            {
                var selectFunction = new Func<DbConnection, Task<TRole>>(async x =>
                {
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("Name", roleName);

                    var query = _sqlConfiguration.SelectRoleByNameQuery
                                                 .ReplaceQueryParameters(_sqlConfiguration.SchemaName,
                                                                         _sqlConfiguration.RoleTable,
                                                                         _sqlConfiguration.ParameterNotation,
                                                                         new string[] { "%NAME%" },
                                                                         new string[] { "Name" });

                    return await x.QueryFirstOrDefaultAsync<TRole>(sql: query,
                                                                   param: dynamicParameters,
                                                                   transaction: _unitOfWork.Transaction);
                });

                DbConnection conn = null;
                if (_unitOfWork?.Connection == null)
                {
                    using (conn = _connectionProvider.Create())
                    {
                        await conn.OpenAsync();

                        return await selectFunction(conn);
                    }
                }
                else
                {
                    conn = _unitOfWork.CreateOrGetConnection();
                    return await selectFunction(conn);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return null;
            }
        }

        public async Task<bool> Insert(TRole role, CancellationToken cancellationToken)
        {
            try
            {
                var insertFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    var columnsBuilder = new StringBuilder();
                    var dynamicParameters = new DynamicParameters(role);

                    var roleProperties = role.GetType()
                                           .GetPublicPropertiesNames(y => !y.Name.Equals("Id"))
                                           .Select(y => string.Concat("\"", y, "\""));

                    var valuesArray = new List<string>(roleProperties.Count());

                    if (!role.Id.Equals(default(TKey)))
                    {
                        columnsBuilder.Append("\"Id\", ");
                        valuesArray.Add($"{_sqlConfiguration.ParameterNotation}Id, ");
                    }

                    columnsBuilder.Append(string.Join(",", roleProperties));
                    valuesArray = valuesArray.InsertQueryValuesFragment(_sqlConfiguration.ParameterNotation, roleProperties);

                    var query = _sqlConfiguration.InsertRoleQuery
                                                 .ReplaceInsertQueryParameters(_sqlConfiguration.SchemaName,
                                                                               _sqlConfiguration.RoleTable,
                                                                               columnsBuilder.ToString(),
                                                                               string.Join(", ", valuesArray));

                    var result = await x.ExecuteAsync(query, dynamicParameters, _unitOfWork.Transaction);

                    return result > 0;
                });

                DbConnection conn = null;
                if (_unitOfWork?.Connection == null)
                {
                    using (conn = _connectionProvider.Create())
                    {
                        await conn.OpenAsync(cancellationToken);

                        return await insertFunction(conn);
                    }
                }
                else
                {
                    conn = _unitOfWork.CreateOrGetConnection();
                    return await insertFunction(conn);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);
                return false;
            }
        }

        public async Task<bool> Remove(TKey id, CancellationToken cancellationToken)
        {
            try
            {
                var removeFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("Id", id);

                    var query = _sqlConfiguration.DeleteRoleQuery
                                                 .ReplaceDeleteQueryParameters(_sqlConfiguration.SchemaName,
                                                                               _sqlConfiguration.RoleTable,
                                                                               $"{_sqlConfiguration.ParameterNotation}Id");

                    var result = await x.ExecuteAsync(query, dynamicParameters, _unitOfWork.Transaction);

                    return result > 0;
                });

                DbConnection conn = null;
                if (_unitOfWork?.Connection == null)
                {
                    using (conn = _connectionProvider.Create())
                    {
                        await conn.OpenAsync(cancellationToken);

                        return await removeFunction(conn);
                    }
                }
                else
                {
                    conn = _unitOfWork.CreateOrGetConnection();
                    return await removeFunction(conn);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);
                return false;
            }
        }

        public async Task<bool> Update(TRole role, CancellationToken cancellationToken)
        {
            try
            {
                var updateFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    var dynamicParameters = new DynamicParameters(role);

                    var roleProperties = role.GetType()
                                             .GetPublicPropertiesNames(y => !y.Name.Equals("Id"))
                                             .Select(y => string.Concat("\"", y, "\""));

                    var setFragment = roleProperties.UpdateQuerySetFragment(_sqlConfiguration.ParameterNotation);

                    var query = _sqlConfiguration.UpdateRoleQuery
                                                 .ReplaceUpdateQueryParameters(_sqlConfiguration.SchemaName,
                                                                               _sqlConfiguration.RoleTable,
                                                                               setFragment,
                                                                               $"{_sqlConfiguration.ParameterNotation}Id");

                    var result = await x.ExecuteAsync(query, dynamicParameters, _unitOfWork.Transaction);

                    return result > 0;
                });

                DbConnection conn = null;
                if (_unitOfWork?.Connection == null)
                {
                    using (conn = _connectionProvider.Create())
                    {
                        await conn.OpenAsync(cancellationToken);
                        return await updateFunction(conn);
                    }
                }
                else
                {
                    conn = _unitOfWork.CreateOrGetConnection();
                    return await updateFunction(conn);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);
                return false;
            }
        }
    }
}
