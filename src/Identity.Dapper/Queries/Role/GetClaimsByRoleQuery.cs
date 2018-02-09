using Identity.Dapper.Models;
using Identity.Dapper.Queries.Contracts;
using System;

namespace Identity.Dapper.Queries.Role
{
    public class GetClaimsByRoleQuery : ISelectQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;
        public GetClaimsByRoleQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery()
        {
            var query = _sqlConfiguration.SelectClaimByRoleQuery
                                         .ReplaceQueryParameters(_sqlConfiguration.SchemaName,
                                                                 string.Empty,
                                                                 _sqlConfiguration.ParameterNotation,
                                                                 new string[] {
                                                                                  "%ROLEID%"
                                                                              },
                                                                 new string[] {
                                                                                  "RoleId"
                                                                              },
                                                                 new string[] {
                                                                                  "%ROLETABLE%",
                                                                                  "%ROLECLAIMTABLE%"
                                                                              },
                                                                 new string[] {
                                                                                  _sqlConfiguration.RoleTable,
                                                                                  _sqlConfiguration.RoleClaimTable
                                                                              }
                                                                );

            return query;
        }

        public string GetQuery<TEntity>(TEntity entity) => throw new NotImplementedException();
    }
}
