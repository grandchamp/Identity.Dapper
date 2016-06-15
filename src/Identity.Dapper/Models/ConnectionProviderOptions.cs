using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.Dapper.Models
{
    public class ConnectionProviderOptions
    {
        public string ConnectionString { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
