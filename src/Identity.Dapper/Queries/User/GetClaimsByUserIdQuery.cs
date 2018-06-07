using Identity.Dapper.Models;
using Identity.Dapper.Queries.Contracts;
using System;

namespace Identity.Dapper.Queries.User
{
    public class GetClaimsByUserIdQuery : ISelectQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;
        public GetClaimsByUserIdQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery()
        {
            var query = _sqlConfiguration.GetClaimsByUserIdQuery
                                         .ReplaceQueryParameters(_sqlConfiguration.SchemaName,
                                                                 _sqlConfiguration.UserClaimTable,
                                                                 _sqlConfiguration.ParameterNotation,
                                                                 new string[] { "%ID%" },
                                                                 new string[] { "UserId" });

            return query;
        }

        public string GetQuery<TEntity>(TEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
