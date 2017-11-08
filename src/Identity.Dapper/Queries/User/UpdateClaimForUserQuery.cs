using Identity.Dapper.Models;
using Identity.Dapper.Queries.Contracts;

namespace Identity.Dapper.Queries.User
{
    public class UpdateClaimForUserQuery : IUpdateQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;
        public UpdateClaimForUserQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery<TEntity>(TEntity entity)
        {
            var query = _sqlConfiguration.UpdateClaimForUserQuery
                                         .ReplaceQueryParameters(_sqlConfiguration.SchemaName,
                                                                 _sqlConfiguration.UserClaimTable,
                                                                 _sqlConfiguration.ParameterNotation,
                                                                 new string[] {
                                                                                 "%NEWCLAIMTYPE%",
                                                                                 "%NEWCLAIMVALUE%",
                                                                                 "%USERID%",
                                                                                 "%CLAIMTYPE%",
                                                                                 "%CLAIMVALUE%"
                                                                              },
                                                                 new string[] {
                                                                                 "NewClaimType",
                                                                                 "NewClaimValue",
                                                                                 "UserId",
                                                                                 "ClaimType",
                                                                                 "ClaimValue"
                                                                              }
                                                                 );

            return query;
        }
    }
}
