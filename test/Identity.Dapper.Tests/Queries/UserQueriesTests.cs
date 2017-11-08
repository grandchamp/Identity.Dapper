using Identity.Dapper.Entities;
using Identity.Dapper.Factories;
using Identity.Dapper.Factories.Contracts;
using Identity.Dapper.Queries;
using Identity.Dapper.Queries.User;
using Identity.Dapper.SqlServer.Models;
using Identity.Dapper.Tests.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Identity.Dapper.Tests.Queries
{
    public class UserQueriesTests
    {
        private readonly IQueryFactory _queryFactory;
        public UserQueriesTests()
        {
            var services = new ServiceCollection();
            services.AddIdentity<DapperIdentityUser, DapperIdentityRole>(x =>
            {
                x.Password.RequireDigit = false;
                x.Password.RequiredLength = 1;
                x.Password.RequireLowercase = false;
                x.Password.RequireNonAlphanumeric = false;
                x.Password.RequireUppercase = false;
            })
                   .AddDapperIdentityFor<SqlServerConfiguration>();

            var serviceProvider = services.BuildServiceProvider();

            var queryList = new QueryList(serviceProvider);

            _queryFactory = new QueryFactory(queryList);
        }

        [Fact]
        public void DeleteUserQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetDeleteQuery<DeleteUserQuery>();
            var expected = "DELETE FROM [dbo].[IdentityUser] WHERE [Id] = @Id";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void GetClaimsByUserIdQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<GetClaimsByUserIdQuery>();
            var expected = "SELECT [ClaimType], [ClaimValue] FROM [dbo].[IdentityUserClaim] WHERE [UserId] = @UserId";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void GetRolesByUserIdQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<GetRolesByUserIdQuery>();
            var expected = "SELECT [Name] FROM [dbo].[IdentityRole], [dbo].[IdentityUserRole] WHERE [UserId] = @UserId AND [dbo].[IdentityRole].[Id] = [dbo].[IdentityUserRole].[RoleId]";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void GetUserLoginByLoginProviderAndProviderKeyQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<GetUserLoginByLoginProviderAndProviderKeyQuery, DapperIdentityUser>(new DapperIdentityUser());
            var expected = "SELECT TOP 1 [IdentityUser].[AccessFailedCount], [IdentityUser].[Email], [IdentityUser].[EmailConfirmed], [IdentityUser].[LockoutEnabled], [IdentityUser].[LockoutEnd], [IdentityUser].[PasswordHash], [IdentityUser].[PhoneNumber], [IdentityUser].[PhoneNumberConfirmed], [IdentityUser].[SecurityStamp], [IdentityUser].[TwoFactorEnabled], [IdentityUser].[UserName] FROM [dbo].[IdentityUser], [dbo].[IdentityLogin] WHERE [dbo].[IdentityUser].[Id] = [dbo].[IdentityLogin].[UserId] AND [LoginProvider] = @LoginProvider AND [ProviderKey] = @ProviderKey";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void GetUserLoginByLoginProviderAndProviderKeyQueryGeneratesCorrectQueryWhenUsingCustomUser()
        {
            var generatedQuery = _queryFactory.GetQuery<GetUserLoginByLoginProviderAndProviderKeyQuery, CustomDapperIdentityUser>(new CustomDapperIdentityUser());
            var expected = "SELECT TOP 1 [IdentityUser].[AccessFailedCount], [IdentityUser].[Dummy], [IdentityUser].[Email], [IdentityUser].[EmailConfirmed], [IdentityUser].[LockoutEnabled], [IdentityUser].[LockoutEnd], [IdentityUser].[PasswordHash], [IdentityUser].[PhoneNumber], [IdentityUser].[PhoneNumberConfirmed], [IdentityUser].[SecurityStamp], [IdentityUser].[TwoFactorEnabled], [IdentityUser].[UserName] FROM [dbo].[IdentityUser], [dbo].[IdentityLogin] WHERE [dbo].[IdentityUser].[Id] = [dbo].[IdentityLogin].[UserId] AND [LoginProvider] = @LoginProvider AND [ProviderKey] = @ProviderKey";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void GetUserLoginInfoByIdQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<GetUserLoginInfoByIdQuery>();
            var expected = "SELECT [LoginProvider], [ProviderKey], [ProviderDisplayName] FROM [dbo].[IdentityLogin] WHERE [UserId] = @UserId";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void GetUsersByClaimQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<GetUsersByClaimQuery, DapperIdentityUser>(new DapperIdentityUser());
            var expected = "SELECT [IdentityUser].[AccessFailedCount], [IdentityUser].[Email], [IdentityUser].[EmailConfirmed], [IdentityUser].[LockoutEnabled], [IdentityUser].[LockoutEnd], [IdentityUser].[PasswordHash], [IdentityUser].[PhoneNumber], [IdentityUser].[PhoneNumberConfirmed], [IdentityUser].[SecurityStamp], [IdentityUser].[TwoFactorEnabled], [IdentityUser].[UserName] FROM [dbo].[IdentityUser], [dbo].[IdentityUserClaim] WHERE [ClaimValue] = @ClaimValue AND [ClaimType] = @ClaimType";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void GetUsersByClaimQueryGeneratesCorrectQueryWhenUsingCustomUser()
        {
            var generatedQuery = _queryFactory.GetQuery<GetUsersByClaimQuery, CustomDapperIdentityUser>(new CustomDapperIdentityUser());
            var expected = "SELECT [IdentityUser].[AccessFailedCount], [IdentityUser].[Dummy], [IdentityUser].[Email], [IdentityUser].[EmailConfirmed], [IdentityUser].[LockoutEnabled], [IdentityUser].[LockoutEnd], [IdentityUser].[PasswordHash], [IdentityUser].[PhoneNumber], [IdentityUser].[PhoneNumberConfirmed], [IdentityUser].[SecurityStamp], [IdentityUser].[TwoFactorEnabled], [IdentityUser].[UserName] FROM [dbo].[IdentityUser], [dbo].[IdentityUserClaim] WHERE [ClaimValue] = @ClaimValue AND [ClaimType] = @ClaimType";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void GetUsersInRoleQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<GetUsersInRoleQuery, DapperIdentityUser>(new DapperIdentityUser());
            var expected = "SELECT [IdentityUser].[AccessFailedCount], [IdentityUser].[Email], [IdentityUser].[EmailConfirmed], [IdentityUser].[LockoutEnabled], [IdentityUser].[LockoutEnd], [IdentityUser].[PasswordHash], [IdentityUser].[PhoneNumber], [IdentityUser].[PhoneNumberConfirmed], [IdentityUser].[SecurityStamp], [IdentityUser].[TwoFactorEnabled], [IdentityUser].[UserName] FROM [dbo].[IdentityUser], [dbo].[IdentityUserRole], [dbo].[IdentityRole] WHERE [dbo].[IdentityRole].[Name] = @RoleName AND [dbo].[IdentityUserRole].[RoleId] = [dbo].[IdentityRole].[Id] AND [dbo].[IdentityUserRole].[UserId] = [dbo].[IdentityUser].[Id]";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void GetUsersInRoleQueryGeneratesCorrectQueryWhenUsingCustomUser()
        {
            var generatedQuery = _queryFactory.GetQuery<GetUsersInRoleQuery, CustomDapperIdentityUser>(new CustomDapperIdentityUser());
            var expected = "SELECT [IdentityUser].[AccessFailedCount], [IdentityUser].[Dummy], [IdentityUser].[Email], [IdentityUser].[EmailConfirmed], [IdentityUser].[LockoutEnabled], [IdentityUser].[LockoutEnd], [IdentityUser].[PasswordHash], [IdentityUser].[PhoneNumber], [IdentityUser].[PhoneNumberConfirmed], [IdentityUser].[SecurityStamp], [IdentityUser].[TwoFactorEnabled], [IdentityUser].[UserName] FROM [dbo].[IdentityUser], [dbo].[IdentityUserRole], [dbo].[IdentityRole] WHERE [dbo].[IdentityRole].[Name] = @RoleName AND [dbo].[IdentityUserRole].[RoleId] = [dbo].[IdentityRole].[Id] AND [dbo].[IdentityUserRole].[UserId] = [dbo].[IdentityUser].[Id]";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void InsertUserClaimQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetInsertQuery<InsertUserClaimQuery, DapperIdentityUserClaim<int>>(new DapperIdentityUserClaim<int>());

            var expected = "INSERT INTO [dbo].[IdentityUserClaim] ([ClaimType], [ClaimValue], [UserId]) VALUES(@ClaimType, @ClaimValue, @UserId)";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void InsertUserClaimQueryGeneratesCorrectQueryWhenProvidingId()
        {
            var generatedQuery = _queryFactory.GetInsertQuery<InsertUserClaimQuery, DapperIdentityUserClaim<int>>(new DapperIdentityUserClaim<int> { Id = 123 });

            var expected = "INSERT INTO [dbo].[IdentityUserClaim] ([ClaimType], [ClaimValue], [Id], [UserId]) VALUES(@ClaimType, @ClaimValue, @Id, @UserId)";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void InsertUserLoginQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetInsertQuery<InsertUserLoginQuery, DapperIdentityUserLogin<int>>(new DapperIdentityUserLogin<int>());

            var expected = "INSERT INTO [dbo].[IdentityLogin] ([LoginProvider], [ProviderDisplayName], [ProviderKey], [UserId]) VALUES(@LoginProvider, @ProviderDisplayName, @ProviderKey, @UserId)";

            Assert.Equal(expected, generatedQuery);
        }
        
    }
}
