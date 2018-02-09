using Identity.Dapper.Models;
using Identity.Dapper.Queries.Contracts;

namespace Identity.Dapper.Queries.Role
{
    public class DeleteRoleClaimQuery : IDeleteQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;
        public DeleteRoleClaimQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery()
        {
            var query = _sqlConfiguration.DeleteRoleClaimQuery
                                         .ReplaceQueryParameters(_sqlConfiguration.SchemaName,
                                                                 _sqlConfiguration.RoleClaimTable,
                                                                 _sqlConfiguration.ParameterNotation,
                                                                 new string[]
                                                                 {
                                                                     "%ROLEID%",
                                                                     "%CLAIMVALUE%",
                                                                     "%CLAIMTYPE%"
                                                                 },
                                                                 new string[]
                                                                 {
                                                                     "RoleId",
                                                                     "ClaimValue",
                                                                     "ClaimType"
                                                                 });

            return query;
        }
    }
}
