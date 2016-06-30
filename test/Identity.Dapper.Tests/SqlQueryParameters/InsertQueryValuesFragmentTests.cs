using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Identity.Dapper.Tests.SqlQueryParameters
{
    public class InsertQueryValuesFragmentTests
    {
        [Fact]
        public void InsertQueryValuesFragmentTest()
        {
            var values = new List<string>();
            values.Add("\"A\"");
            values.Add("\"B\"");
            values.Add("\"C\"");
            values.Add("\"D\"");

            var expected = new string[]
            {
                "@A", "@B", "@C", "@D"
            }.ToList();

            var resultValues = new List<string>();
            resultValues.InsertQueryValuesFragment("@", new string[] { "\"A\"", "\"B\"", "C", "D" });

            Assert.Equal(expected, resultValues);
        }
    }
}
