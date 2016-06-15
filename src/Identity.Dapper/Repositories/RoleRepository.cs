using Dapper;
using Identity.Dapper.Connections;
using Identity.Dapper.Entities;
using Identity.Dapper.Models;
using Identity.Dapper.Repositories.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private readonly IOptions<SqlConfiguration> _sqlConfiguration;
        public RoleRepository(IConnectionProvider connProv, ILogger<RoleRepository<TRole, TKey, TUserRole, TRoleClaim>> log, IOptions<SqlConfiguration> sqlConf)
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

                    var query = _sqlConfiguration.Value.SelectRoleByIdQuery.ReplaceQueryParameters(_sqlConfiguration.Value.SchemaName,
                                                                                                   _sqlConfiguration.Value.RoleTable,
                                                                                                   _sqlConfiguration.Value.ParameterNotation,
                                                                                                   new string[] { "%ID%" },
                                                                                                   new string[] { "Id" });
                    return await conn.QuerySingleAsync<TRole>(sql: query,
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

                    var query = _sqlConfiguration.Value.SelectRoleByNameQuery.ReplaceQueryParameters(_sqlConfiguration.Value.SchemaName,
                                                                                                     _sqlConfiguration.Value.RoleTable,
                                                                                                     _sqlConfiguration.Value.ParameterNotation,
                                                                                                     new string[] { "%NAME%" },
                                                                                                     new string[] { "Name" });
                    return await conn.QuerySingleAsync<TRole>(sql: query,
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
                                           .GetPublicPropertiesNames(x => !x.Name.Equals("Id"));

                    var valuesArray = new List<string>(roleProperties.Count());

                    if (!role.Id.Equals(default(TKey)))
                    {
                        columnsBuilder.Append("Id, ");
                        valuesArray.Add($"{_sqlConfiguration.Value.ParameterNotation}Id, ");
                    }

                    columnsBuilder.Append(string.Join(",", roleProperties));
                    valuesArray = valuesArray.InsertQueryValuesFragment(_sqlConfiguration.Value.ParameterNotation, roleProperties);

                    var query = _sqlConfiguration.Value.InsertRoleQuery.ReplaceInsertQueryParameters(_sqlConfiguration.Value.SchemaName,
                                                                                                     _sqlConfiguration.Value.RoleTable,
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

                    var query = _sqlConfiguration.Value.DeleteRoleQuery.ReplaceDeleteQueryParameters(_sqlConfiguration.Value.SchemaName,
                                                                                                     _sqlConfiguration.Value.RoleTable,
                                                                                                     $"{_sqlConfiguration.Value.ParameterNotation}Id");

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
                                             .GetPublicPropertiesNames(x => !x.Name.Equals("Id"));

                    var setFragment = roleProperties.UpdateQuerySetFragment(_sqlConfiguration.Value.ParameterNotation);

                    var query = _sqlConfiguration.Value.UpdateRoleQuery.ReplaceUpdateQueryParameters(_sqlConfiguration.Value.SchemaName,
                                                                                                     _sqlConfiguration.Value.RoleTable,
                                                                                                     setFragment,
                                                                                                     $"{_sqlConfiguration.Value.ParameterNotation}Id");

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
