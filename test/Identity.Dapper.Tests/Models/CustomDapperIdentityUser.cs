using Identity.Dapper.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Dapper.Tests.Models
{
    public class CustomDapperIdentityUser : DapperIdentityUser
    {
        public string Dummy { get; set; }
    }
}
