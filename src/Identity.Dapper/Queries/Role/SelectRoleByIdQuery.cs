using Identity.Dapper.Models;
using Identity.Dapper.Queries.Contracts;

namespace Identity.Dapper.Queries.Role
{
    public class SelectRoleByIdQuery : ISelectQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;
        public SelectRoleByIdQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery()
        {
            var query = _sqlConfiguration.SelectRoleByIdQuery
                                         .ReplaceQueryParameters(_sqlConfiguration.SchemaName,
                                                                 _sqlConfiguration.RoleTable,
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
