using Identity.Dapper.Models;
using Identity.Dapper.Queries.Contracts;

namespace Identity.Dapper.Queries.User
{
    public class SelectUserByEmailQuery : ISelectQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;
        public SelectUserByEmailQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery()
        {
            var query = _sqlConfiguration.SelectUserByEmailQuery
                                         .ReplaceQueryParameters(_sqlConfiguration.SchemaName,
                                                                 _sqlConfiguration.UserTable,
                                                                 _sqlConfiguration.ParameterNotation,
                                                                 new string[] { "%EMAIL%" },
                                                                 new string[] { "Email" });

            return query;
        }

        public string GetQuery<TEntity>(TEntity entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
