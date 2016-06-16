using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Identity.Dapper.Models;
using Identity.Dapper.Repositories.Contracts;
using Identity.Dapper.Repositories;
using Identity.Dapper.Entities;
using Microsoft.AspNetCore.Identity;
using Identity.Dapper.Stores;
using Identity.Dapper.SqlServer.Connections;
using Identity.Dapper.Connections;
using Identity.Dapper.Cryptography;
using Identity.Dapper.SqlServer.Models;

namespace Identity.Dapper.SqlServer
{
    public static class ServiceCollectionExtensions
    {
        public static IdentityBuilder AddDapperIdentityForSqlServer(this IdentityBuilder builder)
        {
            builder.Services.AddSingleton<SqlConfiguration, SqlServerConfiguration>();

            AddRepositoriesAndStores(builder.Services);

            return builder;
        }

        public static IdentityBuilder AddDapperIdentityForSqlServer(this IdentityBuilder builder, SqlServerConfiguration configurationOverride)
        {
            builder.Services.AddSingleton<SqlConfiguration>(configurationOverride);
            
            AddRepositoriesAndStores(builder.Services);

            return builder;
        }

        private static void AddRepositoriesAndStores(IServiceCollection services)
        {
            #region Repositories Configuration

            services.AddScoped<IRoleRepository<DapperIdentityRole<int>, int, DapperIdentityUserRole<int>, DapperIdentityRoleClaim<int>>,
                                       RoleRepository<DapperIdentityRole<int>, int, DapperIdentityUserRole<int>, DapperIdentityRoleClaim<int>>>();

            services.AddScoped<IUserRepository<DapperIdentityUser, int, DapperIdentityUserRole<int>, DapperIdentityRoleClaim<int>>,
                                       UserRepository<DapperIdentityUser, int, DapperIdentityUserRole<int>, DapperIdentityRoleClaim<int>>>();

            #endregion

            #region Identity Stores Configuration

            services.AddScoped<IRoleStore<DapperIdentityRole<int>>,
                                       DapperRoleStore<DapperIdentityRole<int>, int>>();

            services.AddScoped<IUserStore<DapperIdentityUser>,
                                       DapperUserStore<DapperIdentityUser, int>>();

            #endregion
        }

        public static IServiceCollection ConfigureDapperSqlServerConnectionProvider(this IServiceCollection services, IConfigurationSection configuration)
        {
            services.Configure<ConnectionProviderOptions>(configuration);

            services.AddSingleton<IConnectionProvider, SqlServerConnectionProvider>();

            return services;
        }

        public static IServiceCollection ConfigureDapperIdentityCryptography(this IServiceCollection services, IConfigurationSection configuration)
        {
            services.Configure<AESKeys>(configuration);
            services.AddSingleton<EncryptionHelper>();

            return services;
        }
    }
}
