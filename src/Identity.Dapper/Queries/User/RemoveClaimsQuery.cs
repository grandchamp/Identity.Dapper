using Identity.Dapper.Models;
using Identity.Dapper.Queries.Contracts;

namespace Identity.Dapper.Queries.User
{
    public class RemoveClaimsQuery : IDeleteQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;
        public RemoveClaimsQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery()
        {
            var query = _sqlConfiguration.RemoveClaimsQuery
                                         .ReplaceQueryParameters(_sqlConfiguration.SchemaName,
                                                                 _sqlConfiguration.UserClaimTable,
                                                                 _sqlConfiguration.ParameterNotation,
                                                                 new string[] {
                                                                                 "%ID%",
                                                                                 "%CLAIMVALUE%",
                                                                                 "%CLAIMTYPE%"
                                                                              },
                                                                 new string[] {
                                                                                 "UserId",
                                                                                 "ClaimValue",
                                                                                 "ClaimType"
                                                                              }
                                                                 );

            return query;
        }
    }
}
