using Identity.Dapper.Models;
using Identity.Dapper.Queries.Contracts;
using System;

namespace Identity.Dapper.Queries.User
{
    public class GetUserLoginByLoginProviderAndProviderKeyQuery : ISelectQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;
        public GetUserLoginByLoginProviderAndProviderKeyQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery()
        {
            throw new NotImplementedException();
        }

        public string GetQuery<TEntity>(TEntity entity)
        {
            var userProperties = entity.GetColumns(_sqlConfiguration, ignoreIdProperty: false, ignoreProperties: new string[] { "ConcurrencyStamp" }, forInsert: false);

            var query = _sqlConfiguration.GetUserLoginByLoginProviderAndProviderKeyQuery
                                         .ReplaceQueryParameters(_sqlConfiguration.SchemaName,
                                                                 string.Empty,
                                                                 _sqlConfiguration.ParameterNotation,
                                                                 new string[] {
                                                                                        "%LOGINPROVIDER%",
                                                                                        "%PROVIDERKEY%"
                                                                              },
                                                                 new string[] {
                                                                                        "LoginProvider",
                                                                                        "ProviderKey"
                                                                              },
                                                                 new string[] {
                                                                                        "%USERFILTER%",
                                                                                        "%USERTABLE%",
                                                                                        "%USERLOGINTABLE%",
                                                                                        "%USERROLETABLE%"
                                                                              },
                                                                 new string[] {
                                                                                        userProperties.SelectFilterWithTableName(_sqlConfiguration.UserTable),
                                                                                        _sqlConfiguration.UserTable,
                                                                                        _sqlConfiguration.UserLoginTable,
                                                                                        _sqlConfiguration.UserRoleTable
                                                                              }
                                                                 );

            return query;
        }
    }
}
