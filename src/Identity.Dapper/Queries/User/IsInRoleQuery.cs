using Identity.Dapper.Models;
using Identity.Dapper.Queries.Contracts;
using System;

namespace Identity.Dapper.Queries.User
{
    public class IsInRoleQuery : ISelectQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;
        public IsInRoleQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery()
        {
            throw new NotImplementedException();
        }

        public string GetQuery<TEntity>(TEntity entity)
        {
            var userProperties = entity.GetColumns(_sqlConfiguration, ignoreIdProperty: true, ignoreProperties: new string[] { "ConcurrencyStamp" });

            var query = _sqlConfiguration.IsInRoleQuery
                                         .ReplaceQueryParameters(_sqlConfiguration.SchemaName,
                                                                 _sqlConfiguration.UserTable,
                                                                 _sqlConfiguration.ParameterNotation,
                                                                 new string[] {
                                                                                        "%ROLENAME%",
                                                                                        "%USERID%"
                                                                              },
                                                                 new string[] {
                                                                                        "RoleName",
                                                                                        "UserId"
                                                                              },
                                                                 new string[] {
                                                                                        "%USERTABLE%",
                                                                                        "%USERROLETABLE%",
                                                                                        "%ROLETABLE%"
                                                                              },
                                                                 new string[] {
                                                                                        _sqlConfiguration.UserTable,
                                                                                        _sqlConfiguration.UserRoleTable,
                                                                                        _sqlConfiguration.RoleTable
                                                                              }
                                                                 );

            return query;
        }
    }
}
