using Identity.Dapper.Models;
using Identity.Dapper.Queries.Contracts;

namespace Identity.Dapper.Queries.User
{
    public class SelectUserByIdQuery : ISelectQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;
        public SelectUserByIdQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery()
        {
            var query = _sqlConfiguration.SelectUserByIdQuery
                                         .ReplaceQueryParameters(_sqlConfiguration.SchemaName,
                                                                 _sqlConfiguration.UserTable,
                                                                 _sqlConfiguration.ParameterNotation,
                                                                 new string[] { "%ID%" },
                                                                 new string[] { "Id" });

            return query;
        }

        public string GetQuery<TEntity>(TEntity entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
