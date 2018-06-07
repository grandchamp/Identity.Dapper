using Identity.Dapper.Models;
using Identity.Dapper.Queries.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Identity.Dapper.Queries.User
{
    public class InsertUserClaimQuery : IInsertQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;
        public InsertUserClaimQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery<TEntity>(TEntity entity)
        {
            var columns = entity.GetColumns(_sqlConfiguration);

            var valuesArray = new List<string>(columns.Count());
            valuesArray = valuesArray.InsertQueryValuesFragment(_sqlConfiguration.ParameterNotation, columns);

            var query = _sqlConfiguration.InsertUserClaimQuery
                                         .ReplaceInsertQueryParameters(_sqlConfiguration.SchemaName,
                                                                       _sqlConfiguration.UserClaimTable,
                                                                       columns.GetCommaSeparatedColumns(),
                                                                       string.Join(", ", valuesArray));

            return query;
        }
    }
}
