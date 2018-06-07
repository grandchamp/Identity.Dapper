using Identity.Dapper.Connections;
using Identity.Dapper.Entities;
using Identity.Dapper.Models;
using Identity.Dapper.SqlServer.Connections;
using Identity.Dapper.SqlServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using Xunit;

namespace Identity.Dapper.Tests.Integration
{
    public class ServiceCollectionTests
    {
        private readonly IServiceCollection _serviceCollection;
        public ServiceCollectionTests()
        {
            _serviceCollection = new ServiceCollection();
        }

        [Fact]
        public void ConfigureDapperConnectionProviderWithDapperIdentitySection()
        {
            var builder = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("dapperidentityconfig.json", optional: true, reloadOnChange: true)
                                .Build();

            _serviceCollection.AddLogging();

            _serviceCollection.ConfigureDapperConnectionProvider<SqlServerConnectionProvider>(builder.GetSection("DapperIdentity"))
                              .ConfigureDapperIdentityCryptography(builder.GetSection("DapperIdentityCryptography"))
                              .ConfigureDapperIdentityOptions(new DapperIdentityOptions { UseTransactionalBehavior = false }); //Change to True to use Transactions in all operations

            _serviceCollection.AddIdentity<DapperIdentityUser, DapperIdentityRole>(x =>
            {
                x.Password.RequireDigit = false;
                x.Password.RequiredLength = 1;
                x.Password.RequireLowercase = false;
                x.Password.RequireNonAlphanumeric = false;
                x.Password.RequireUppercase = false;
            })
                              .AddDapperIdentityFor<SqlServerConfiguration>()
                              .AddDefaultTokenProviders();

            var serviceProvider = _serviceCollection.BuildServiceProvider(false);

            var provider = serviceProvider.GetService<IConnectionProvider>();
            var options = serviceProvider.GetService<IOptions<ConnectionProviderOptions>>();

            Assert.NotNull(provider);
            Assert.NotNull(options);
        }

        [Fact]
        public void ConfigureDapperConnectionProviderWithConnectionStringsSectionWithDefaultConnection()
        {
            var builder = new ConfigurationBuilder()
                               .SetBasePath(Directory.GetCurrentDirectory())
                               .AddJsonFile("connectionstringswithdefaultconnectionconfig.json", optional: true, reloadOnChange: true)
                               .Build();

            _serviceCollection.AddLogging();

            _serviceCollection.ConfigureDapperConnectionProvider<SqlServerConnectionProvider>(builder.GetSection("ConnectionStrings"))
                              .ConfigureDapperIdentityCryptography(builder.GetSection("DapperIdentityCryptography"))
                              .ConfigureDapperIdentityOptions(new DapperIdentityOptions { UseTransactionalBehavior = false }); //Change to True to use Transactions in all operations

            _serviceCollection.AddIdentity<DapperIdentityUser, DapperIdentityRole>(x =>
            {
                x.Password.RequireDigit = false;
                x.Password.RequiredLength = 1;
                x.Password.RequireLowercase = false;
                x.Password.RequireNonAlphanumeric = false;
                x.Password.RequireUppercase = false;
            })
                              .AddDapperIdentityFor<SqlServerConfiguration>()
                              .AddDefaultTokenProviders();

            var serviceProvider = _serviceCollection.BuildServiceProvider(false);

            var provider = serviceProvider.GetService<IConnectionProvider>();
            var options = serviceProvider.GetService<IOptions<ConnectionProviderOptions>>();

            Assert.NotNull(provider);
            Assert.NotNull(options);
        }

        [Fact]
        public void ConfigureDapperConnectionProviderWithConnectionStringsSectionWithoutDefaultConnection()
        {
            var builder = new ConfigurationBuilder()
                               .SetBasePath(Directory.GetCurrentDirectory())
                               .AddJsonFile("connectionstringswithoutdefaultconnectionconfig.json", optional: true, reloadOnChange: true)
                               .Build();

            _serviceCollection.AddLogging();

            _serviceCollection.ConfigureDapperConnectionProvider<SqlServerConnectionProvider>(builder.GetSection("ConnectionStrings"))
                              .ConfigureDapperIdentityCryptography(builder.GetSection("DapperIdentityCryptography"))
                              .ConfigureDapperIdentityOptions(new DapperIdentityOptions { UseTransactionalBehavior = false }); //Change to True to use Transactions in all operations

            _serviceCollection.AddIdentity<DapperIdentityUser, DapperIdentityRole>(x =>
            {
                x.Password.RequireDigit = false;
                x.Password.RequiredLength = 1;
                x.Password.RequireLowercase = false;
                x.Password.RequireNonAlphanumeric = false;
                x.Password.RequireUppercase = false;
            })
                              .AddDapperIdentityFor<SqlServerConfiguration>()
                              .AddDefaultTokenProviders();

            var serviceProvider = _serviceCollection.BuildServiceProvider(false);

            var provider = serviceProvider.GetService<IConnectionProvider>();
            var options = serviceProvider.GetService<IOptions<ConnectionProviderOptions>>();

            Assert.NotNull(provider);
            Assert.NotNull(options);
        }

        [Fact]
        public void ConfigureDapperConnectionProviderWithoutConnectionStringOrDapperIdentitySectionThrowsException()
        {
            var builder = new ConfigurationBuilder()
                              .SetBasePath(Directory.GetCurrentDirectory())
                              .AddJsonFile("emptyconfig.json", optional: true, reloadOnChange: true)
                              .Build();

            _serviceCollection.AddLogging();

            Assert.Throws<Exception>(() => _serviceCollection.ConfigureDapperConnectionProvider<SqlServerConnectionProvider>(builder.GetSection("ConnectionStrings")));
        }
    }
}
