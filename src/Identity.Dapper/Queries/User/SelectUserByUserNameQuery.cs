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
                                                                 _sqlConfiguration.UserTable,
                                                                 _sqlConfiguration.ParameterNotation,
                                                                 new string[] { "%USERNAME%" },
                                                                 new string[] { "User" });

            return query;
        }

        public string GetQuery<TEntity>(TEntity entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
