using Identity.Dapper.Entities;
using Identity.Dapper.Factories;
using Identity.Dapper.Factories.Contracts;
using Identity.Dapper.Queries;
using Identity.Dapper.Queries.User;
using Identity.Dapper.SqlServer.Models;
using Identity.Dapper.Tests.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Identity.Dapper.Tests.Queries.SQLServer
{
    public class SqlServerUserQueriesTests
    {
        private readonly IQueryFactory _queryFactory;
        public SqlServerUserQueriesTests()
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
            const string expected = "DELETE FROM [dbo].[IdentityUser] WHERE [Id] = @Id";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void GetClaimsByUserIdQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<GetClaimsByUserIdQuery>();
            const string expected = "SELECT [ClaimType], [ClaimValue] FROM [dbo].[IdentityUserClaim] WHERE [UserId] = @UserId";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void GetRolesByUserIdQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<GetRolesByUserIdQuery>();
            const string expected = "SELECT [Name] FROM [dbo].[IdentityRole], [dbo].[IdentityUserRole] WHERE [UserId] = @UserId AND [dbo].[IdentityRole].[Id] = [dbo].[IdentityUserRole].[RoleId]";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void GetUserLoginByLoginProviderAndProviderKeyQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<GetUserLoginByLoginProviderAndProviderKeyQuery, DapperIdentityUser>(new DapperIdentityUser());
            const string expected = "SELECT TOP 1 [IdentityUser].[AccessFailedCount], [IdentityUser].[Email], [IdentityUser].[EmailConfirmed], [IdentityUser].[Id], [IdentityUser].[LockoutEnabled], [IdentityUser].[LockoutEnd], [IdentityUser].[PasswordHash], [IdentityUser].[PhoneNumber], [IdentityUser].[PhoneNumberConfirmed], [IdentityUser].[SecurityStamp], [IdentityUser].[TwoFactorEnabled], [IdentityUser].[UserName], [dbo].[IdentityUserRole].* FROM [dbo].[IdentityUser] LEFT JOIN [dbo].[IdentityUserRole] ON [dbo].[IdentityUserRole].[UserId] = [dbo].[IdentityUser].[Id] INNER JOIN [dbo].[IdentityLogin] ON [dbo].[IdentityUser].[Id] = [dbo].[IdentityLogin].[UserId] WHERE [LoginProvider] = @LoginProvider AND [ProviderKey] = @ProviderKey";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void GetUserLoginByLoginProviderAndProviderKeyQueryGeneratesCorrectQueryWhenUsingCustomUser()
        {
            var generatedQuery = _queryFactory.GetQuery<GetUserLoginByLoginProviderAndProviderKeyQuery, CustomDapperIdentityUser>(new CustomDapperIdentityUser());
            const string expected = "SELECT TOP 1 [IdentityUser].[AccessFailedCount], [IdentityUser].[Dummy], [IdentityUser].[Email], [IdentityUser].[EmailConfirmed], [IdentityUser].[Id], [IdentityUser].[LockoutEnabled], [IdentityUser].[LockoutEnd], [IdentityUser].[PasswordHash], [IdentityUser].[PhoneNumber], [IdentityUser].[PhoneNumberConfirmed], [IdentityUser].[SecurityStamp], [IdentityUser].[TwoFactorEnabled], [IdentityUser].[UserName], [dbo].[IdentityUserRole].* FROM [dbo].[IdentityUser] LEFT JOIN [dbo].[IdentityUserRole] ON [dbo].[IdentityUserRole].[UserId] = [dbo].[IdentityUser].[Id] INNER JOIN [dbo].[IdentityLogin] ON [dbo].[IdentityUser].[Id] = [dbo].[IdentityLogin].[UserId] WHERE [LoginProvider] = @LoginProvider AND [ProviderKey] = @ProviderKey";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void GetUserLoginInfoByIdQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<GetUserLoginInfoByIdQuery>();
            const string expected = "SELECT [LoginProvider], [Name], [ProviderKey] FROM [dbo].[IdentityLogin] WHERE [UserId] = @UserId";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void GetUsersByClaimQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<GetUsersByClaimQuery, DapperIdentityUser>(new DapperIdentityUser());
            const string expected = "SELECT [IdentityUser].[AccessFailedCount], [IdentityUser].[Email], [IdentityUser].[EmailConfirmed], [IdentityUser].[LockoutEnabled], [IdentityUser].[LockoutEnd], [IdentityUser].[PasswordHash], [IdentityUser].[PhoneNumber], [IdentityUser].[PhoneNumberConfirmed], [IdentityUser].[SecurityStamp], [IdentityUser].[TwoFactorEnabled], [IdentityUser].[UserName] FROM [dbo].[IdentityUser], [dbo].[IdentityUserClaim] WHERE [ClaimValue] = @ClaimValue AND [ClaimType] = @ClaimType";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void GetUsersByClaimQueryGeneratesCorrectQueryWhenUsingCustomUser()
        {
            var generatedQuery = _queryFactory.GetQuery<GetUsersByClaimQuery, CustomDapperIdentityUser>(new CustomDapperIdentityUser());
            const string expected = "SELECT [IdentityUser].[AccessFailedCount], [IdentityUser].[Dummy], [IdentityUser].[Email], [IdentityUser].[EmailConfirmed], [IdentityUser].[LockoutEnabled], [IdentityUser].[LockoutEnd], [IdentityUser].[PasswordHash], [IdentityUser].[PhoneNumber], [IdentityUser].[PhoneNumberConfirmed], [IdentityUser].[SecurityStamp], [IdentityUser].[TwoFactorEnabled], [IdentityUser].[UserName] FROM [dbo].[IdentityUser], [dbo].[IdentityUserClaim] WHERE [ClaimValue] = @ClaimValue AND [ClaimType] = @ClaimType";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void GetUsersInRoleQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<GetUsersInRoleQuery, DapperIdentityUser>(new DapperIdentityUser());
            const string expected = "SELECT [IdentityUser].[AccessFailedCount], [IdentityUser].[Email], [IdentityUser].[EmailConfirmed], [IdentityUser].[LockoutEnabled], [IdentityUser].[LockoutEnd], [IdentityUser].[PasswordHash], [IdentityUser].[PhoneNumber], [IdentityUser].[PhoneNumberConfirmed], [IdentityUser].[SecurityStamp], [IdentityUser].[TwoFactorEnabled], [IdentityUser].[UserName] FROM [dbo].[IdentityUser], [dbo].[IdentityUserRole], [dbo].[IdentityRole] WHERE [dbo].[IdentityRole].[Name] = @RoleName AND [dbo].[IdentityUserRole].[RoleId] = [dbo].[IdentityRole].[Id] AND [dbo].[IdentityUserRole].[UserId] = [dbo].[IdentityUser].[Id]";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void GetUsersInRoleQueryGeneratesCorrectQueryWhenUsingCustomUser()
        {
            var generatedQuery = _queryFactory.GetQuery<GetUsersInRoleQuery, CustomDapperIdentityUser>(new CustomDapperIdentityUser());
            const string expected = "SELECT [IdentityUser].[AccessFailedCount], [IdentityUser].[Dummy], [IdentityUser].[Email], [IdentityUser].[EmailConfirmed], [IdentityUser].[LockoutEnabled], [IdentityUser].[LockoutEnd], [IdentityUser].[PasswordHash], [IdentityUser].[PhoneNumber], [IdentityUser].[PhoneNumberConfirmed], [IdentityUser].[SecurityStamp], [IdentityUser].[TwoFactorEnabled], [IdentityUser].[UserName] FROM [dbo].[IdentityUser], [dbo].[IdentityUserRole], [dbo].[IdentityRole] WHERE [dbo].[IdentityRole].[Name] = @RoleName AND [dbo].[IdentityUserRole].[RoleId] = [dbo].[IdentityRole].[Id] AND [dbo].[IdentityUserRole].[UserId] = [dbo].[IdentityUser].[Id]";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void InsertUserClaimQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetInsertQuery<InsertUserClaimQuery, DapperIdentityUserClaim<int>>(new DapperIdentityUserClaim<int>());

            const string expected = "INSERT INTO [dbo].[IdentityUserClaim] ([ClaimType], [ClaimValue], [UserId]) VALUES(@ClaimType, @ClaimValue, @UserId)";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void InsertUserClaimQueryGeneratesCorrectQueryWhenProvidingId()
        {
            var generatedQuery = _queryFactory.GetInsertQuery<InsertUserClaimQuery, DapperIdentityUserClaim<int>>(new DapperIdentityUserClaim<int> { Id = 123 });

            const string expected = "INSERT INTO [dbo].[IdentityUserClaim] ([ClaimType], [ClaimValue], [Id], [UserId]) VALUES(@ClaimType, @ClaimValue, @Id, @UserId)";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void InsertUserLoginQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetInsertQuery<InsertUserLoginQuery, DapperIdentityUserLogin<int>>(new DapperIdentityUserLogin<int>());

            const string expected = "INSERT INTO [dbo].[IdentityLogin] ([LoginProvider], [ProviderDisplayName], [ProviderKey], [UserId]) VALUES(@LoginProvider, @ProviderDisplayName, @ProviderKey, @UserId)";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void InsertUserQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetInsertQuery<InsertUserQuery, DapperIdentityUser>(new DapperIdentityUser());

            const string expected = "INSERT INTO [dbo].[IdentityUser] ([AccessFailedCount], [Email], [EmailConfirmed], [LockoutEnabled], [LockoutEnd], [PasswordHash], [PhoneNumber], [PhoneNumberConfirmed], [SecurityStamp], [TwoFactorEnabled], [UserName]) OUTPUT INSERTED.Id VALUES(@AccessFailedCount, @Email, @EmailConfirmed, @LockoutEnabled, @LockoutEnd, @PasswordHash, @PhoneNumber, @PhoneNumberConfirmed, @SecurityStamp, @TwoFactorEnabled, @UserName)";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void InsertUserQueryGeneratesCorrectQueryWhenUsingCustomUser()
        {
            var generatedQuery = _queryFactory.GetInsertQuery<InsertUserQuery, CustomDapperIdentityUser>(new CustomDapperIdentityUser());

            const string expected = "INSERT INTO [dbo].[IdentityUser] ([AccessFailedCount], [Dummy], [Email], [EmailConfirmed], [LockoutEnabled], [LockoutEnd], [PasswordHash], [PhoneNumber], [PhoneNumberConfirmed], [SecurityStamp], [TwoFactorEnabled], [UserName]) OUTPUT INSERTED.Id VALUES(@AccessFailedCount, @Dummy, @Email, @EmailConfirmed, @LockoutEnabled, @LockoutEnd, @PasswordHash, @PhoneNumber, @PhoneNumberConfirmed, @SecurityStamp, @TwoFactorEnabled, @UserName)";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void InsertUserRoleQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetInsertQuery<InsertUserRoleQuery, DapperIdentityUserRole<int>>(new DapperIdentityUserRole<int>());

            const string expected = "INSERT INTO [dbo].[IdentityUserRole] ([RoleId], [UserId]) VALUES(@RoleId, @UserId)";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void IsInRoleQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<IsInRoleQuery, DapperIdentityUser>(new DapperIdentityUser());

            const string expected = "SELECT 1 FROM [dbo].[IdentityUser], [dbo].[IdentityUserRole], [dbo].[IdentityRole] WHERE [dbo].[IdentityRole].[Name] = @RoleName AND [dbo].[IdentityUser].[Id] = @UserId AND [dbo].[IdentityUserRole].[RoleId] = [dbo].[IdentityRole].[Id] AND [dbo].[IdentityUserRole].[UserId] = [dbo].[IdentityUser].[Id]";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void RemoveClaimsQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetDeleteQuery<RemoveClaimsQuery>();

            const string expected = "DELETE FROM [dbo].[IdentityUserClaim] WHERE [UserId] = @UserId AND [ClaimType] = @ClaimType AND [ClaimValue] = @ClaimValue";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void RemoveLoginForUserQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetDeleteQuery<RemoveLoginForUserQuery>();

            const string expected = "DELETE FROM [dbo].[IdentityLogin] WHERE [UserId] = @UserId AND [LoginProvider] = @LoginProvider AND [ProviderKey] = @ProviderKey";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void RemoveUserFromRoleQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetDeleteQuery<RemoveUserFromRoleQuery>();

            const string expected = "DELETE FROM [dbo].[IdentityUserRole] WHERE [UserId] = @UserId AND [RoleId] = (SELECT [Id] FROM [dbo].[IdentityRole] WHERE [Name] = @RoleName)";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void SelectUserByEmailQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<SelectUserByEmailQuery>();

            const string expected = "SELECT [dbo].[IdentityUser].*, [dbo].[IdentityUserRole].* FROM [dbo].[IdentityUser] LEFT JOIN [dbo].[IdentityUserRole] ON [dbo].[IdentityUserRole].[UserId] =  [dbo].[IdentityUser].[Id] WHERE [Email] = @Email";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void SelectUserByIdQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<SelectUserByIdQuery>();

            const string expected = "SELECT [dbo].[IdentityUser].*, [dbo].[IdentityUserRole].* FROM [dbo].[IdentityUser] LEFT JOIN [dbo].[IdentityUserRole] ON [dbo].[IdentityUserRole].[UserId] =  [dbo].[IdentityUser].[Id] WHERE [Id] = @Id";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void SelectUserByUserNameQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<SelectUserByUserNameQuery>();

            const string expected = "SELECT [dbo].[IdentityUser].*, [dbo].[IdentityUserRole].* FROM [dbo].[IdentityUser] LEFT JOIN [dbo].[IdentityUserRole] ON [dbo].[IdentityUserRole].[UserId] =  [dbo].[IdentityUser].[Id] WHERE [UserName] = @UserName";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void UpdateClaimForUserQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetUpdateQuery<UpdateClaimForUserQuery, DapperIdentityUserClaim<int>>(new DapperIdentityUserClaim<int>());

            const string expected = "UPDATE [dbo].[IdentityUserClaim] SET [ClaimType] = @NewClaimType, [ClaimValue] = @NewClaimValue WHERE [UserId] = @UserId AND [ClaimType] = @ClaimType AND [ClaimValue] = @ClaimValue";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void UpdateUserQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetUpdateQuery<UpdateUserQuery, DapperIdentityUser>(new DapperIdentityUser());

            const string expected = "UPDATE [dbo].[IdentityUser] SET [AccessFailedCount] = @AccessFailedCount, [Email] = @Email, [EmailConfirmed] = @EmailConfirmed, [LockoutEnabled] = @LockoutEnabled, [LockoutEnd] = @LockoutEnd, [PasswordHash] = @PasswordHash, [PhoneNumber] = @PhoneNumber, [PhoneNumberConfirmed] = @PhoneNumberConfirmed, [SecurityStamp] = @SecurityStamp, [TwoFactorEnabled] = @TwoFactorEnabled, [UserName] = @UserName WHERE [Id] = @Id";

            Assert.Equal(expected, generatedQuery);
        }
    }
}
