//using Xunit;

//namespace Identity.Dapper.Tests
//{
//    public class ReplaceDeleteQueryParametersTests
//    {
//        [Fact]
//        public void ReplaceDeleteQueryParametersTest()
//        {
//            const string query = "DELETE FROM %SCHEMA%.%TABLENAME% WHERE Id = %ID%";
//            const string expectedQuery = "DELETE FROM dbo.IdentityUser WHERE Id = @Id";

//            Assert.Equal(expectedQuery,
//                         query.ReplaceDeleteQueryParameters("dbo",
//                                                            "IdentityUser",
//                                                            "@Id"));
//        }
//    }
//}
