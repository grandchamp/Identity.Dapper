using Identity.Dapper.Entities;

namespace Identity.Dapper.Tests.Models
{
    public class CustomDapperIdentityRole : DapperIdentityRole<int>
    {
        public string Dummy { get; set; }
    }
}
