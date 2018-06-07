using Identity.Dapper.Models;
using Identity.Dapper.Queries.Contracts;

namespace Identity.Dapper.Queries.Role
{
    public class DeleteRoleQuery : IDeleteQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;
        public DeleteRoleQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery()
        {
            var query = _sqlConfiguration.DeleteRoleQuery
                                         .ReplaceDeleteQueryParameters(_sqlConfiguration.SchemaName,
                                                                       _sqlConfiguration.RoleTable,
                                                                       $"{_sqlConfiguration.ParameterNotation}Id");

            return query;
        }
    }
}
