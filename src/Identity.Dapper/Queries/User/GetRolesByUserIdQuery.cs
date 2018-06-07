using Identity.Dapper.Models;
using Identity.Dapper.Queries.Contracts;
using System;

namespace Identity.Dapper.Queries.User
{
    public class GetRolesByUserIdQuery : ISelectQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;
        public GetRolesByUserIdQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery()
        {
            var query = _sqlConfiguration.GetRolesByUserIdQuery
                                         .ReplaceQueryParameters(_sqlConfiguration.SchemaName,
                                                                 string.Empty,
                                                                 _sqlConfiguration.ParameterNotation,
                                                                 new string[] {
                                                                                  "%ID%"
                                                                              },
                                                                 new string[] {
                                                                                  "UserId"
                                                                              },
                                                                 new string[] {
                                                                                  "%ROLETABLE%",
                                                                                  "%USERROLETABLE%"
                                                                              },
                                                                 new string[] {
                                                                                  _sqlConfiguration.RoleTable,
                                                                                  _sqlConfiguration.UserRoleTable
                                                                              }
                                                                );

            return query;
        }

        public string GetQuery<TEntity>(TEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
