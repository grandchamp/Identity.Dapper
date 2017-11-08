using Identity.Dapper.Models;
using Identity.Dapper.Queries.Contracts;

namespace Identity.Dapper.Queries.Role
{
    public class SelectRoleByNameQuery : ISelectQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;
        public SelectRoleByNameQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery()
        {
            var query = _sqlConfiguration.SelectRoleByNameQuery
                                         .ReplaceQueryParameters(_sqlConfiguration.SchemaName,
                                                                 _sqlConfiguration.RoleTable,
                                                                 _sqlConfiguration.ParameterNotation,
                                                                 new string[] { "%NAME%" },
                                                                 new string[] { "Name" });

            return query;
        }

        public string GetQuery<TEntity>(TEntity entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
