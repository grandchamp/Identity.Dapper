using Identity.Dapper.Models;
using Identity.Dapper.Queries.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Identity.Dapper.Queries.Role
{
    public class InsertRoleQuery : IInsertQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;
        public InsertRoleQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery<TEntity>(TEntity entity)
        {
            var columns = entity.GetColumns(_sqlConfiguration);

            var valuesArray = new List<string>(columns.Count());
            valuesArray = valuesArray.InsertQueryValuesFragment(_sqlConfiguration.ParameterNotation, columns);

            var query = _sqlConfiguration.InsertRoleQuery
                                         .ReplaceInsertQueryParameters(_sqlConfiguration.SchemaName,
                                                                       _sqlConfiguration.RoleTable,
                                                                       columns.GetCommaSeparatedColumns(),
                                                                       string.Join(", ", valuesArray));

            return query;
        }
    }
}
