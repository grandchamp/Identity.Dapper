using Dapper;
using Identity.Dapper.Connections;
using Identity.Dapper.MySQL.Connections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Dapper.Tests.Integration.MySQL
{
    public class MySqlDatabaseFixture
    {
        public TestServer TestServer { get; set; }
        public MySqlDatabaseFixture()
        {
            var builder = new WebHostBuilder().UseStartup<TestStartupMySql>();
            TestServer = new TestServer(builder);
        }

        public void Dispose()
        {
            TestServer.Dispose();
        }
    }
}
