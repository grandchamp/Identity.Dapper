using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Identity.Dapper.Tests.SqlQueryParameters
{
    public class UpdateQuerySetFragmentTests
    {
        [Fact]
        public void UpdateQuerySetFragmentTest()
        {
            var values = new string[]
            {
                "A",
                "\"B\"",
                "C",
                "\"D\""
            };

            var expected = "SET A = @A, B = @B, C = @C, D = @D";

            Assert.Equal(expected,
                         values.UpdateQuerySetFragment("@"));
        }
    }
}
