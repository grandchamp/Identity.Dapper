using Dapper;
using Identity.Dapper.Connections;
using Identity.Dapper.Entities;
using Identity.Dapper.Models;
using Identity.Dapper.Repositories.Contracts;
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
    public class RoleRepository<TRole, TKey, TUserRole, TRoleClaim> : IRoleRepository<TRole, TKey, TUserRole, TRoleClaim>
        where TRole : DapperIdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserRole : DapperIdentityUserRole<TKey>
        where TRoleClaim : DapperIdentityRoleClaim<TKey>
    {
        private readonly IConnectionProvider _connectionProvider;
        private readonly ILogger<RoleRepository<TRole, TKey, TUserRole, TRoleClaim>> _log;
        private readonly SqlConfiguration _sqlConfiguration;
        public RoleRepository(IConnectionProvider connProv, ILogger<RoleRepository<TRole, TKey, TUserRole, TRoleClaim>> log, SqlConfiguration sqlConf)
        {
            _connectionProvider = connProv;
            _log = log;
            _sqlConfiguration = sqlConf;
        }

        public async Task<TRole> GetById(TKey id)
        {
            try
            {
                using (var conn = _connectionProvider.Create())
                {
                    await conn.OpenAsync();

                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("Id", id);

                    var query = _sqlConfiguration.SelectRoleByIdQuery
                                                 .ReplaceQueryParameters(_sqlConfiguration.SchemaName,
                                                                         _sqlConfiguration.RoleTable,
                                                                         _sqlConfiguration.ParameterNotation,
                                                                         new string[] { "%ID%" },
                                                                         new string[] { "Id" });

                    return await conn.QueryFirstOrDefaultAsync<TRole>(sql: query,
                                                                      param: dynamicParameters);
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
                using (var conn = _connectionProvider.Create())
                {
                    await conn.OpenAsync();

                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("Name", roleName);

                    var query = _sqlConfiguration.SelectRoleByNameQuery
                                                 .ReplaceQueryParameters(_sqlConfiguration.SchemaName,
                                                                         _sqlConfiguration.RoleTable,
                                                                         _sqlConfiguration.ParameterNotation,
                                                                         new string[] { "%NAME%" },
                                                                         new string[] { "Name" });

                    return await conn.QueryFirstOrDefaultAsync<TRole>(sql: query,
                                                                      param: dynamicParameters);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return null;
            }
        }

        public async Task<bool> Insert(TRole role, CancellationToken cancellationToken, DbTransaction transaction = null)
        {
            try
            {
                using (var conn = _connectionProvider.Create())
                {
                    await conn.OpenAsync();

                    var columnsBuilder = new StringBuilder();
                    var dynamicParameters = new DynamicParameters(role);

                    var roleProperties = role.GetType()
                                           .GetPublicPropertiesNames(x => !x.Name.Equals("Id"))
                                           .Select(x => string.Concat("\"", x, "\""));

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

                    var result = await conn.ExecuteAsync(query, dynamicParameters);

                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);
                return false;
            }
        }

        public async Task<bool> Remove(TKey id, CancellationToken cancellationToken, DbTransaction transaction = null)
        {
            try
            {
                using (var conn = _connectionProvider.Create())
                {
                    await conn.OpenAsync();

                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("Id", id);

                    var query = _sqlConfiguration.DeleteRoleQuery
                                                 .ReplaceDeleteQueryParameters(_sqlConfiguration.SchemaName,
                                                                               _sqlConfiguration.RoleTable,
                                                                               $"{_sqlConfiguration.ParameterNotation}Id");

                    var result = await conn.ExecuteAsync(query, dynamicParameters);

                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);
                return false;
            }
        }

        public async Task<bool> Update(TRole role, CancellationToken cancellationToken, DbTransaction transaction = null)
        {
            try
            {
                using (var conn = _connectionProvider.Create())
                {
                    await conn.OpenAsync();

                    var dynamicParameters = new DynamicParameters(role);

                    var roleProperties = role.GetType()
                                             .GetPublicPropertiesNames(x => !x.Name.Equals("Id"))
                                             .Select(x => string.Concat("\"", x, "\""));

                    var setFragment = roleProperties.UpdateQuerySetFragment(_sqlConfiguration.ParameterNotation);

                    var query = _sqlConfiguration.UpdateRoleQuery
                                                 .ReplaceUpdateQueryParameters(_sqlConfiguration.SchemaName,
                                                                               _sqlConfiguration.RoleTable,
                                                                               setFragment,
                                                                               $"{_sqlConfiguration.ParameterNotation}Id");

                    var result = await conn.ExecuteAsync(query, dynamicParameters);

                    return result > 0;
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
