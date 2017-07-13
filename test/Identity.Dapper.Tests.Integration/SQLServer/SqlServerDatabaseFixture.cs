using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Dapper.Tests.Integration.SQLServer
{
    public class SqlServerDatabaseFixture
    {
        public TestServer TestServer { get; set; }
        public SqlServerDatabaseFixture()
        {
            var builder = new WebHostBuilder().UseStartup<TestStartupSqlServer>();
            TestServer = new TestServer(builder);
        }

        public void Dispose()
        {
            TestServer.Dispose();
        }
    }
}
