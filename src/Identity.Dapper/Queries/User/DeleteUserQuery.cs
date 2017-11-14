using Identity.Dapper.Models;
using Identity.Dapper.Queries.Contracts;

namespace Identity.Dapper.Queries.User
{
    public class DeleteUserQuery : IDeleteQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;
        public DeleteUserQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery()
        {
            var query = _sqlConfiguration.DeleteUserQuery
                                         .ReplaceDeleteQueryParameters(_sqlConfiguration.SchemaName,
                                                                       _sqlConfiguration.UserTable,
                                                                       $"{_sqlConfiguration.ParameterNotation}Id");
                          
            return query;
        }
    }
}
