using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Identity.Dapper.Tests
{
    public class ReplaceUpdateQueryParametersTests
    {
        [Fact]
        public void ReplaceUpdateQueryParametersTest()
        {
            const string query = "UPDATE %SCHEMA%.%TABLENAME% %SETVALUES% WHERE Id = %ID%";
            const string expectedQuery = "UPDATE dbo.IdentityUser SET VALUES WHERE Id = @Id";

            Assert.Equal(expectedQuery,
                         query.ReplaceUpdateQueryParameters("dbo",
                                                            "IdentityUser",
                                                            "SET VALUES",
                                                            "@Id"));
        }
    }
}
