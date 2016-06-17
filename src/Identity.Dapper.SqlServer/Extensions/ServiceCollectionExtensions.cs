using Identity.Dapper.Connections;
using Identity.Dapper.Cryptography;
using Identity.Dapper.Entities;
using Identity.Dapper.Models;
using Identity.Dapper.Repositories;
using Identity.Dapper.Repositories.Contracts;
using Identity.Dapper.SqlServer.Connections;
using Identity.Dapper.SqlServer.Models;
using Identity.Dapper.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Identity.Dapper.SqlServer
{
    public static class ServiceCollectionExtensions
    {
        public static IdentityBuilder AddDapperIdentityForSqlServer(this IdentityBuilder builder)
        {
            builder.Services.AddSingleton<SqlConfiguration, SqlServerConfiguration>();

            AddStores(builder.Services, builder.UserType, builder.RoleType);

            return builder;
        }

        public static IdentityBuilder AddDapperIdentityForSqlServer(this IdentityBuilder builder, SqlServerConfiguration configurationOverride)
        {
            builder.Services.AddSingleton<SqlConfiguration>(configurationOverride);

            AddStores(builder.Services, builder.UserType, builder.RoleType);

            return builder;
        }

        public static IdentityBuilder AddDapperIdentityForSqlServer<TKey>(this IdentityBuilder builder)
        {
            builder.Services.AddSingleton<SqlConfiguration, SqlServerConfiguration>();

            AddStores(builder.Services, builder.UserType, builder.RoleType, typeof(TKey));

            return builder;
        }

        public static IdentityBuilder AddDapperIdentityForSqlServer<TKey>(this IdentityBuilder builder, SqlServerConfiguration configurationOverride)
        {
            builder.Services.AddSingleton<SqlConfiguration>(configurationOverride);

            AddStores(builder.Services, builder.UserType, builder.RoleType, typeof(TKey));

            return builder;
        }

        public static IdentityBuilder AddDapperIdentityForSqlServer<TKey, TUserRole, TRoleClaim>(this IdentityBuilder builder)
        {
            builder.Services.AddSingleton<SqlConfiguration, SqlServerConfiguration>();

            AddStores(builder.Services, builder.UserType, builder.RoleType, typeof(TKey), typeof(TUserRole), typeof(TRoleClaim));

            return builder;
        }

        public static IdentityBuilder AddDapperIdentityForSqlServer<TKey, TUserRole, TRoleClaim>(this IdentityBuilder builder, SqlServerConfiguration configurationOverride)
        {
            builder.Services.AddSingleton<SqlConfiguration>(configurationOverride);

            AddStores(builder.Services, builder.UserType, builder.RoleType, typeof(TKey), typeof(TUserRole), typeof(TRoleClaim));

            return builder;
        }

        private static void AddStores(IServiceCollection services, Type userType, Type roleType, Type keyType = null, Type userRoleType = null, Type roleClaimType = null)
        {
            Type userStoreType;
            Type roleStoreType;
            keyType = keyType ?? typeof(int);
            userRoleType = userRoleType ?? typeof(DapperIdentityUserRole<>).MakeGenericType(keyType);
            roleClaimType = roleClaimType ?? typeof(DapperIdentityRoleClaim<>).MakeGenericType(keyType);

            userStoreType = typeof(DapperUserStore<,,,>).MakeGenericType(userType, keyType, userRoleType, roleClaimType);
            roleStoreType = typeof(DapperRoleStore<,,,>).MakeGenericType(roleType, keyType, userRoleType, roleClaimType);

            services.AddScoped(typeof(IRoleRepository<,,,>).MakeGenericType(roleType, keyType, userRoleType, roleClaimType),
                               typeof(RoleRepository<,,,>).MakeGenericType(roleType, keyType, userRoleType, roleClaimType));

            services.AddScoped(typeof(IUserRepository<,,,>).MakeGenericType(userType, keyType, userRoleType, roleClaimType),
                               typeof(UserRepository<,,,>).MakeGenericType(userType, keyType, userRoleType, roleClaimType));

            services.AddScoped(typeof(IUserStore<>).MakeGenericType(userType), userStoreType);
            services.AddScoped(typeof(IRoleStore<>).MakeGenericType(roleType), roleStoreType);
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
