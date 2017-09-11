using System;
using Identity.Dapper.Connections;
using Identity.Dapper.Cryptography;
using Identity.Dapper.Entities;
using Identity.Dapper.Models;
using Identity.Dapper.Repositories;
using Identity.Dapper.Repositories.Contracts;
using Identity.Dapper.Stores;
using Identity.Dapper.UnitOfWork.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Dapper
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureDapperIdentityCryptography(this IServiceCollection services, IConfigurationSection configuration)
        {
            services.Configure<AESKeys>(configuration);
            services.AddSingleton<EncryptionHelper>();

            return services;
        }

        public static IServiceCollection ConfigureDapperIdentityOptions(this IServiceCollection services, DapperIdentityOptions options)
        {
            services.AddSingleton(options);

            return services;
        }

        public static IdentityBuilder AddDapperIdentity<TSqlConfiguration>(this IdentityBuilder builder)
        {
            builder.Services.AddSingleton(typeof(SqlConfiguration), typeof(TSqlConfiguration));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

            AddStores(builder.Services, builder.UserType, builder.RoleType);

            return builder;
        }

        public static IdentityBuilder AddDapperIdentity<TSqlConfiguration, TKey>(this IdentityBuilder builder)
        {
            builder.Services.AddSingleton(typeof(SqlConfiguration), typeof(TSqlConfiguration));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

            AddStores(builder.Services, builder.UserType, builder.RoleType, typeof(TKey));

            return builder;
        }

        public static IdentityBuilder AddDapperIdentity<TSqlConfiguration, TKey, TUserRole, TRoleClaim>(this IdentityBuilder builder)
        {
            builder.Services.AddSingleton(typeof(SqlConfiguration), typeof(TSqlConfiguration));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

            AddStores(builder.Services, builder.UserType, builder.RoleType, typeof(TKey), typeof(TUserRole), typeof(TRoleClaim));

            return builder;
        }

        private static void AddStores(IServiceCollection services, Type userType, Type roleType, Type keyType = null, Type userRoleType = null, Type roleClaimType = null, Type userClaimType = null, Type userLoginType = null)
        {
            Type userStoreType;
            Type roleStoreType;
            keyType = keyType ?? typeof(int);
            userRoleType = userRoleType ?? typeof(DapperIdentityUserRole<>).MakeGenericType(keyType);
            roleClaimType = roleClaimType ?? typeof(DapperIdentityRoleClaim<>).MakeGenericType(keyType);
            userClaimType = userClaimType ?? typeof(DapperIdentityUserClaim<>).MakeGenericType(keyType);
            userLoginType = userLoginType ?? typeof(DapperIdentityUserLogin<>).MakeGenericType(keyType);

            userStoreType = typeof(DapperUserStore<,,,,,,>).MakeGenericType(userType, keyType, userRoleType, roleClaimType,
                                                                           userClaimType, userLoginType, roleType);
            roleStoreType = typeof(DapperRoleStore<,,,>).MakeGenericType(roleType, keyType, userRoleType, roleClaimType);

            services.AddScoped(typeof(IRoleRepository<,,,>).MakeGenericType(roleType, keyType, userRoleType, roleClaimType),
                               typeof(RoleRepository<,,,>).MakeGenericType(roleType, keyType, userRoleType, roleClaimType));

            services.AddScoped(typeof(IUserRepository<,,,,,,>).MakeGenericType(userType, keyType, userRoleType,
                                                                              roleClaimType, userClaimType,
                                                                              userLoginType, roleType),
                               typeof(UserRepository<,,,,,,>).MakeGenericType(userType, keyType, userRoleType,
                                                                             roleClaimType, userClaimType,
                                                                             userLoginType, roleType));

            services.AddScoped(typeof(IUserStore<>).MakeGenericType(userType), userStoreType);
            services.AddScoped(typeof(IRoleStore<>).MakeGenericType(roleType), roleStoreType);
        }

        public static IServiceCollection ConfigureDapperConnectionProvider<TConnectionProvider>(this IServiceCollection services, IConfigurationSection configuration) where TConnectionProvider : IConnectionProvider
        {
            services.Configure<ConnectionProviderOptions>(configuration);

            services.AddScoped(typeof(IConnectionProvider), typeof(TConnectionProvider));

            return services;
        }
    }
}
