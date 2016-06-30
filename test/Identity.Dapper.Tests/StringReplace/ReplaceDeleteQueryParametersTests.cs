using Xunit;

namespace Identity.Dapper.Tests
{
    public class ReplaceDeleteQueryParametersTests
    {
        [Fact]
        public void ReplaceDeleteQueryParametersTest()
        {
            var query = "DELETE FROM %SCHEMA%.%TABLENAME% WHERE Id = %ID%";
            var expectedQuery = "DELETE FROM dbo.IdentityUser WHERE Id = @Id";

            Assert.Equal(expectedQuery,
                         query.ReplaceDeleteQueryParameters("dbo",
                                                            "IdentityUser",
                                                            "@Id"));
        }
    }
}
