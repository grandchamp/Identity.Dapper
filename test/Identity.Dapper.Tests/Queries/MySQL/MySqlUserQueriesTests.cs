using Identity.Dapper.Entities;
using Identity.Dapper.Factories;
using Identity.Dapper.Factories.Contracts;
using Identity.Dapper.MySQL.Models;
using Identity.Dapper.Queries;
using Identity.Dapper.Queries.User;
using Identity.Dapper.Tests.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Identity.Dapper.Tests.Queries.MySQL
{
    public class MySqlUserQueriesTests
    {
        private readonly IQueryFactory _queryFactory;
        public MySqlUserQueriesTests()
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
                   .AddDapperIdentityFor<MySqlConfiguration>();

            var serviceProvider = services.BuildServiceProvider();

            var queryList = new QueryList(serviceProvider);

            _queryFactory = new QueryFactory(queryList);
        }

        [Fact]
        public void DeleteUserQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetDeleteQuery<DeleteUserQuery>();
            const string expected = "DELETE FROM `identity`.`identityuser` WHERE `Id` = @Id";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void GetClaimsByUserIdQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<GetClaimsByUserIdQuery>();
            const string expected = "SELECT `ClaimType`, `ClaimValue` FROM `identity`.`identityuserclaim` WHERE `UserId` = @UserId";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void GetRolesByUserIdQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<GetRolesByUserIdQuery>();
            const string expected = "SELECT `Name` FROM `identity`.`identityrole`, `identity`.`identityuserrole` WHERE `UserId` = @UserId AND `identity`.`identityuserrole`.`RoleId` = `identity`.`identityrole`.`Id`";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void GetUserLoginByLoginProviderAndProviderKeyQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<GetUserLoginByLoginProviderAndProviderKeyQuery, DapperIdentityUser>(new DapperIdentityUser());
            const string expected = "SELECT `identityuser`.`AccessFailedCount`, `identityuser`.`Email`, `identityuser`.`EmailConfirmed`, `identityuser`.`LockoutEnabled`, `identityuser`.`LockoutEnd`, `identityuser`.`PasswordHash`, `identityuser`.`PhoneNumber`, `identityuser`.`PhoneNumberConfirmed`, `identityuser`.`SecurityStamp`, `identityuser`.`TwoFactorEnabled`, `identityuser`.`UserName` FROM `identity`.`identityuser`, `identity`.`identitylogin` WHERE `identity`.`identityuser`.`Id` = `identity`.`identitylogin`.`UserId` AND `LoginProvider` = @LoginProvider AND `ProviderKey` = @ProviderKey LIMIT 1";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void GetUserLoginByLoginProviderAndProviderKeyQueryGeneratesCorrectQueryWhenUsingCustomUser()
        {
            var generatedQuery = _queryFactory.GetQuery<GetUserLoginByLoginProviderAndProviderKeyQuery, CustomDapperIdentityUser>(new CustomDapperIdentityUser());
            const string expected = "SELECT `identityuser`.`AccessFailedCount`, `identityuser`.`Dummy`, `identityuser`.`Email`, `identityuser`.`EmailConfirmed`, `identityuser`.`LockoutEnabled`, `identityuser`.`LockoutEnd`, `identityuser`.`PasswordHash`, `identityuser`.`PhoneNumber`, `identityuser`.`PhoneNumberConfirmed`, `identityuser`.`SecurityStamp`, `identityuser`.`TwoFactorEnabled`, `identityuser`.`UserName` FROM `identity`.`identityuser`, `identity`.`identitylogin` WHERE `identity`.`identityuser`.`Id` = `identity`.`identitylogin`.`UserId` AND `LoginProvider` = @LoginProvider AND `ProviderKey` = @ProviderKey LIMIT 1";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void GetUserLoginInfoByIdQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<GetUserLoginInfoByIdQuery>();
            const string expected = "SELECT `LoginProvider`, `Name`, `ProviderKey` FROM `identity`.`identitylogin` WHERE `UserId` = @UserId";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void GetUsersByClaimQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<GetUsersByClaimQuery, DapperIdentityUser>(new DapperIdentityUser());
            const string expected = "SELECT `identityuser`.`AccessFailedCount`, `identityuser`.`Email`, `identityuser`.`EmailConfirmed`, `identityuser`.`LockoutEnabled`, `identityuser`.`LockoutEnd`, `identityuser`.`PasswordHash`, `identityuser`.`PhoneNumber`, `identityuser`.`PhoneNumberConfirmed`, `identityuser`.`SecurityStamp`, `identityuser`.`TwoFactorEnabled`, `identityuser`.`UserName` FROM `identity`.`identityuser`, `identity`.`identityuserclaim` WHERE `ClaimValue` = @ClaimValue AND `ClaimType` = @ClaimType";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void GetUsersByClaimQueryGeneratesCorrectQueryWhenUsingCustomUser()
        {
            var generatedQuery = _queryFactory.GetQuery<GetUsersByClaimQuery, CustomDapperIdentityUser>(new CustomDapperIdentityUser());
            const string expected = "SELECT `identityuser`.`AccessFailedCount`, `identityuser`.`Dummy`, `identityuser`.`Email`, `identityuser`.`EmailConfirmed`, `identityuser`.`LockoutEnabled`, `identityuser`.`LockoutEnd`, `identityuser`.`PasswordHash`, `identityuser`.`PhoneNumber`, `identityuser`.`PhoneNumberConfirmed`, `identityuser`.`SecurityStamp`, `identityuser`.`TwoFactorEnabled`, `identityuser`.`UserName` FROM `identity`.`identityuser`, `identity`.`identityuserclaim` WHERE `ClaimValue` = @ClaimValue AND `ClaimType` = @ClaimType";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void GetUsersInRoleQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<GetUsersInRoleQuery, DapperIdentityUser>(new DapperIdentityUser());
            const string expected = "SELECT `identityuser`.`AccessFailedCount`, `identityuser`.`Email`, `identityuser`.`EmailConfirmed`, `identityuser`.`LockoutEnabled`, `identityuser`.`LockoutEnd`, `identityuser`.`PasswordHash`, `identityuser`.`PhoneNumber`, `identityuser`.`PhoneNumberConfirmed`, `identityuser`.`SecurityStamp`, `identityuser`.`TwoFactorEnabled`, `identityuser`.`UserName` FROM `identity`.`identityuser`, `identity`.`identityuserrole`, `identity`.`identityrole` WHERE `identity`.`identityrole`.`Name` = @RoleName AND `identity`.`identityuserrole`.`RoleId` = `identity`.`identityrole`.`Id` AND `identity`.`identityuserrole`.`UserId` = `identity`.`identityuser`.`Id`";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void GetUsersInRoleQueryGeneratesCorrectQueryWhenUsingCustomUser()
        {
            var generatedQuery = _queryFactory.GetQuery<GetUsersInRoleQuery, CustomDapperIdentityUser>(new CustomDapperIdentityUser());
            const string expected = "SELECT `identityuser`.`AccessFailedCount`, `identityuser`.`Dummy`, `identityuser`.`Email`, `identityuser`.`EmailConfirmed`, `identityuser`.`LockoutEnabled`, `identityuser`.`LockoutEnd`, `identityuser`.`PasswordHash`, `identityuser`.`PhoneNumber`, `identityuser`.`PhoneNumberConfirmed`, `identityuser`.`SecurityStamp`, `identityuser`.`TwoFactorEnabled`, `identityuser`.`UserName` FROM `identity`.`identityuser`, `identity`.`identityuserrole`, `identity`.`identityrole` WHERE `identity`.`identityrole`.`Name` = @RoleName AND `identity`.`identityuserrole`.`RoleId` = `identity`.`identityrole`.`Id` AND `identity`.`identityuserrole`.`UserId` = `identity`.`identityuser`.`Id`";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void InsertUserClaimQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetInsertQuery<InsertUserClaimQuery, DapperIdentityUserClaim<int>>(new DapperIdentityUserClaim<int>());

            const string expected = "INSERT INTO `identity`.`identityuserclaim` (`ClaimType`, `ClaimValue`, `UserId`) VALUES(@ClaimType, @ClaimValue, @UserId)";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void InsertUserClaimQueryGeneratesCorrectQueryWhenProvidingId()
        {
            var generatedQuery = _queryFactory.GetInsertQuery<InsertUserClaimQuery, DapperIdentityUserClaim<int>>(new DapperIdentityUserClaim<int> { Id = 123 });

            const string expected = "INSERT INTO `identity`.`identityuserclaim` (`ClaimType`, `ClaimValue`, `Id`, `UserId`) VALUES(@ClaimType, @ClaimValue, @Id, @UserId)";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void InsertUserLoginQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetInsertQuery<InsertUserLoginQuery, DapperIdentityUserLogin<int>>(new DapperIdentityUserLogin<int>());

            const string expected = "INSERT INTO `identity`.`identitylogin` (`LoginProvider`, `ProviderDisplayName`, `ProviderKey`, `UserId`) VALUES(@LoginProvider, @ProviderDisplayName, @ProviderKey, @UserId)";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void InsertUserQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetInsertQuery<InsertUserQuery, DapperIdentityUser>(new DapperIdentityUser());

            const string expected = "INSERT INTO `identity`.`identityuser` (`AccessFailedCount`, `Email`, `EmailConfirmed`, `LockoutEnabled`, `LockoutEnd`, `PasswordHash`, `PhoneNumber`, `PhoneNumberConfirmed`, `SecurityStamp`, `TwoFactorEnabled`, `UserName`) VALUES(@AccessFailedCount, @Email, @EmailConfirmed, @LockoutEnabled, @LockoutEnd, @PasswordHash, @PhoneNumber, @PhoneNumberConfirmed, @SecurityStamp, @TwoFactorEnabled, @UserName);SELECT LAST_INSERT_ID();";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void InsertUserQueryGeneratesCorrectQueryWhenUsingCustomUser()
        {
            var generatedQuery = _queryFactory.GetInsertQuery<InsertUserQuery, CustomDapperIdentityUser>(new CustomDapperIdentityUser());

            const string expected = "INSERT INTO `identity`.`identityuser` (`AccessFailedCount`, `Dummy`, `Email`, `EmailConfirmed`, `LockoutEnabled`, `LockoutEnd`, `PasswordHash`, `PhoneNumber`, `PhoneNumberConfirmed`, `SecurityStamp`, `TwoFactorEnabled`, `UserName`) VALUES(@AccessFailedCount, @Dummy, @Email, @EmailConfirmed, @LockoutEnabled, @LockoutEnd, @PasswordHash, @PhoneNumber, @PhoneNumberConfirmed, @SecurityStamp, @TwoFactorEnabled, @UserName);SELECT LAST_INSERT_ID();";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void InsertUserRoleQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetInsertQuery<InsertUserRoleQuery, DapperIdentityUserRole<int>>(new DapperIdentityUserRole<int>());

            const string expected = "INSERT INTO `identity`.`identityuserrole` (`RoleId`, `UserId`) VALUES(@RoleId, @UserId)";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void IsInRoleQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<IsInRoleQuery, DapperIdentityUser>(new DapperIdentityUser());
            const string expected = "SELECT 1 FROM `identity`.`identityuser`, `identity`.`identityuserrole`, `identity`.`identityrole` WHERE `identity`.`identityrole`.`Name` = @RoleName AND `identity`.`identityuser`.`Id` = @UserId AND `identity`.`identityuserrole`.`RoleId` = `identity`.`identityrole`.`Id` AND `identity`.`identityuserrole`.`UserId` = `identity`.`identityuser`.`Id`";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void RemoveClaimsQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetDeleteQuery<RemoveClaimsQuery>();

            const string expected = "DELETE FROM `identity`.`identityuserclaim` WHERE `UserId` = @UserId AND `ClaimType` = @ClaimType AND `ClaimValue` = @ClaimValue";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void RemoveLoginForUserQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetDeleteQuery<RemoveLoginForUserQuery>();

            const string expected = "DELETE FROM `identity`.`identitylogin` WHERE `UserId` = @UserId AND `LoginProvider` = @LoginProvider AND `ProviderKey` = @ProviderKey";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void RemoveUserFromRoleQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetDeleteQuery<RemoveUserFromRoleQuery>();
            const string expected = "DELETE FROM `identity`.`identityuserrole` WHERE `UserId` = @UserId AND `RoleId` = (SELECT `Id` FROM `identity`.`identityrole` WHERE `Name` = @RoleName)";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void SelectUserByEmailQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<SelectUserByEmailQuery>();

            const string expected = "SELECT `identity`.`identityuser`.*, `identity`.`identityuserrole`.* FROM `identity`.`identityuser` LEFT JOIN `identity`.`identityuserrole` ON `identity`.`identityuserrole`.`UserId` =  `identity`.`identityuser`.`Id` WHERE `Email` = @Email";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void SelectUserByIdQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<SelectUserByIdQuery>();

            const string expected = "SELECT `identity`.`identityuser`.*, `identity`.`identityuserrole`.* FROM `identity`.`identityuser` LEFT JOIN `identity`.`identityuserrole` ON `identity`.`identityuserrole`.`UserId` =  `identity`.`identityuser`.`Id` WHERE `Id` = @Id";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void SelectUserByUserNameQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<SelectUserByUserNameQuery>();

            const string expected = "SELECT `identity`.`identityuser`.*, `identity`.`identityuserrole`.* FROM `identity`.`identityuser` LEFT JOIN `identity`.`identityuserrole` ON `identity`.`identityuserrole`.`UserId` =  `identity`.`identityuser`.`Id` WHERE `UserName` = @UserName";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void UpdateClaimForUserQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetUpdateQuery<UpdateClaimForUserQuery, DapperIdentityUserClaim<int>>(new DapperIdentityUserClaim<int>());

            const string expected = "UPDATE `identity`.`identityuserclaim` SET `ClaimType` = @NewClaimType, `ClaimValue` = @NewClaimValue WHERE `UserId` = @UserId AND `ClaimType` = @ClaimType AND `ClaimValue` = @ClaimValue";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void UpdateUserQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetUpdateQuery<UpdateUserQuery, DapperIdentityUser>(new DapperIdentityUser());

            const string expected = "UPDATE `identity`.`identityuser` SET `AccessFailedCount` = @AccessFailedCount, `Email` = @Email, `EmailConfirmed` = @EmailConfirmed, `LockoutEnabled` = @LockoutEnabled, `LockoutEnd` = @LockoutEnd, `PasswordHash` = @PasswordHash, `PhoneNumber` = @PhoneNumber, `PhoneNumberConfirmed` = @PhoneNumberConfirmed, `SecurityStamp` = @SecurityStamp, `TwoFactorEnabled` = @TwoFactorEnabled, `UserName` = @UserName WHERE `Id` = @Id";

            Assert.Equal(expected, generatedQuery);
        }
    }
}
