using Identity.Dapper.Models;
using Identity.Dapper.Queries.Contracts;

namespace Identity.Dapper.Queries.User
{
    public class SelectUserByUserNameQuery : ISelectQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;
        public SelectUserByUserNameQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery()
        {
            var query = _sqlConfiguration.SelectUserByUserNameQuery
                                         .ReplaceQueryParameters(_sqlConfiguration.SchemaName,
                                                                  string.Empty,
                                                                 _sqlConfiguration.ParameterNotation,
                                                                new string[] {
                                                                                "%USERNAME%"
                                                                              },
                                                                 new string[] {
                                                                                "UserName",
                                                                              },
                                                                 new string[] {
                                                                                "%USERTABLE%",
                                                                                "%ROLETABLE%",
                                                                                "%USERROLETABLE%",
                                                                              },
                                                                 new string[] {
                                                                                _sqlConfiguration.UserTable,
                                                                                _sqlConfiguration.RoleTable,
                                                                                _sqlConfiguration.UserRoleTable,
                                                                              }
                                                                 );

            return query;
        }

        public string GetQuery<TEntity>(TEntity entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
