using Identity.Dapper.Models;
using Identity.Dapper.Queries.Contracts;
using System;

namespace Identity.Dapper.Queries.User
{
    public class GetUserLoginInfoByIdQuery : ISelectQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;
        public GetUserLoginInfoByIdQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery()
        {
            var query = _sqlConfiguration.GetUserLoginInfoByIdQuery
                                         .ReplaceQueryParameters(_sqlConfiguration.SchemaName,
                                                                 _sqlConfiguration.UserLoginTable,
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
