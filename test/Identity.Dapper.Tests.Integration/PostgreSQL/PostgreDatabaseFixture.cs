using Dapper;
using Identity.Dapper.Connections;
using Identity.Dapper.PostgreSQL.Connections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Dapper.Tests.Integration.PostgreSQL
{
    public class PostgreDatabaseFixture : IDisposable
    {
        public TestServer TestServer { get; set; }
        public PostgreDatabaseFixture()
        {
            var builder = new WebHostBuilder().UseStartup<TestStartupPostgreSql>();
            TestServer = new TestServer(builder);
        }

        public void Dispose()
        {
            TestServer.Dispose();
        }
    }
}
