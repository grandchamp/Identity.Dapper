using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Identity.Dapper.Tests
{
    public class ReplaceInsertQueryParametersTests
    {
        [Fact]
        public void ReplaceInsertQueryParametersTest()
        {
            var query = "INSERT INTO %SCHEMA%.%TABLENAME% %COLUMNS% VALUES(%VALUES%)";
            var expected = "INSERT INTO dbo.IdentityUser (A, B, C) VALUES(@A, @B, @C)";

            Assert.Equal(expected,
                         query.ReplaceInsertQueryParameters("dbo",
                                                            "IdentityUser",
                                                            "A, B, C",
                                                            "@A, @B, @C"));
        }
    }
}
