using Identity.Dapper.Models;
using Identity.Dapper.Queries.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Dapper.Queries.User
{
    public class RemoveUserFromRoleQuery : IDeleteQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;
        public RemoveUserFromRoleQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery()
        {
            var query = _sqlConfiguration.RemoveUserFromRoleQuery
                                         .ReplaceQueryParameters(_sqlConfiguration.SchemaName,
                                                                 _sqlConfiguration.UserRoleTable,
                                                                 _sqlConfiguration.ParameterNotation,
                                                                 new string[] {
                                                                                "%USERID%",
                                                                                "%ROLENAME%"
                                                                              },
                                                                 new string[] {
                                                                                "UserId",
                                                                                "RoleName"
                                                                              },
                                                                 new string[] {
                                                                                "%USERROLETABLE%",
                                                                                "%ROLETABLE%"
                                                                              },
                                                                 new string[] {
                                                                                _sqlConfiguration.UserRoleTable,
                                                                                _sqlConfiguration.RoleTable
                                                                              }
                                                                 );

            return query;
        }
    }
}
