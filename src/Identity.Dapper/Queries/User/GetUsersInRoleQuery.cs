using Identity.Dapper.Models;
using Identity.Dapper.Queries.Contracts;
using System;

namespace Identity.Dapper.Queries.User
{
    public class GetUsersInRoleQuery : ISelectQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;
        public GetUsersInRoleQuery(SqlConfiguration sqlConfiguration)
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

            var query = _sqlConfiguration.GetUsersInRoleQuery
                                         .ReplaceQueryParameters(_sqlConfiguration.SchemaName,
                                                                 string.Empty,
                                                                 _sqlConfiguration.ParameterNotation,
                                                                 new string[] {
                                                                                        "%ROLENAME%"
                                                                              },
                                                                 new string[] {
                                                                                        "RoleName"
                                                                              },
                                                                 new string[] {
                                                                                        "%USERFILTER%",
                                                                                        "%USERTABLE%",
                                                                                        "%USERROLETABLE%",
                                                                                        "%ROLETABLE%"
                                                                              },
                                                                 new string[] {
                                                                                        userProperties.SelectFilterWithTableName(_sqlConfiguration.UserTable),
                                                                                        _sqlConfiguration.UserTable,
                                                                                        _sqlConfiguration.UserRoleTable,
                                                                                        _sqlConfiguration.RoleTable
                                                                              }
                                                                 );

            return query;
        }
    }
}
