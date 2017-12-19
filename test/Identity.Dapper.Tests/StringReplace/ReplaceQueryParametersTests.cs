using Xunit;

namespace Identity.Dapper.Tests
{
    public class ReplaceQueryParametersTests
    {
        [Fact]
        public void ReplaceQueryParametersSingleParameter()
        {
            const string query = "SELECT * FROM %SCHEMA%.%TABLENAME% WHERE Id = %ID%";
            const string expectedQuery = "SELECT * FROM dbo.IdentityRole WHERE Id = @Id";

            Assert.Equal(expectedQuery,
                         query.ReplaceQueryParameters("dbo",
                                                      "IdentityRole",
                                                      "@",
                                                      new string[] { "%ID%" },
                                                      new string[] { "Id" }));
        }

        [Fact]
        public void ReplaceQueryParametersMultipleParameters()
        {
            const string query = "DELETE FROM %SCHEMA%.%TABLENAME% WHERE UserId = %USERID% AND LoginProvider = %LOGINPROVIDER% AND ProviderKey = %PROVIDERKEY%";
            const string expectedQuery = "DELETE FROM dbo.IdentityUserLogin WHERE UserId = @UserId AND LoginProvider = @LoginProvider AND ProviderKey = @ProviderKey";

            Assert.Equal(expectedQuery,
                         query.ReplaceQueryParameters("dbo",
                                                      "IdentityUserLogin",
                                                      "@",
                                                      new string[]
                                                      {
                                                          "%USERID%",
                                                          "%LOGINPROVIDER%",
                                                          "%PROVIDERKEY%"
                                                      },
                                                      new string[] {
                                                          "UserId",
                                                          "LoginProvider",
                                                          "ProviderKey"
                                                      }));
        }

        [Fact]
        public void ReplaceQueryParametersWithOthersParametersSingle()
        {
            const string query = "SELECT Name FROM %SCHEMA%.%ROLETABLE%, %SCHEMA%.%USERROLETABLE% WHERE UserId = %ID%";
            const string expectedQuery = "SELECT Name FROM dbo.IdentityRole, dbo.IdentityUserRole WHERE UserId = @Id";

            Assert.Equal(expectedQuery,
                         query.ReplaceQueryParameters("dbo",
                                                      string.Empty,
                                                      "@",
                                                      new string[]
                                                      {
                                                          "%ID%"
                                                      },
                                                      new string[] {
                                                          "Id"
                                                      },
                                                      new string[] {
                                                          "%ROLETABLE%",
                                                          "%USERROLETABLE%"
                                                      },
                                                      new string[] {
                                                          "IdentityRole",
                                                          "IdentityUserRole"
                                                      }));
        }

        [Fact]
        public void ReplaceQueryParametersWithOthersParametersMultiple()
        {
            const string query = "SELECT %USERFILTER% FROM %SCHEMA%.%USERTABLE%, %SCHEMA%.%USERCLAIMTABLE% WHERE ClaimValue = %CLAIMVALUE% AND ClaimType = %CLAIMTYPE%";
            const string expectedQuery = "SELECT Filter FROM dbo.IdentityUser, dbo.IdentityUserClaim WHERE ClaimValue = @ClaimValue AND ClaimType = @ClaimType";

            Assert.Equal(expectedQuery,
                         query.ReplaceQueryParameters("dbo",
                                                      string.Empty,
                                                      "@",
                                                      new string[]
                                                      {
                                                          "%CLAIMVALUE%",
                                                          "%CLAIMTYPE%"
                                                      },
                                                      new string[] {
                                                          "ClaimValue",
                                                          "ClaimType"
                                                      },
                                                      new string[] {
                                                          "%USERFILTER%",
                                                          "%USERTABLE%",
                                                          "%USERCLAIMTABLE%"
                                                      },
                                                      new string[] {
                                                          "Filter",
                                                          "IdentityUser",
                                                          "IdentityUserClaim"
                                                      }));
        }
    }
}
