using Identity.Dapper.Entities;
using Identity.Dapper.Factories;
using Identity.Dapper.Factories.Contracts;
using Identity.Dapper.MySQL.Models;
using Identity.Dapper.PostgreSQL.Models;
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
            var expected = "DELETE FROM \"dbo\".\"IdentityUser\" WHERE \"Id\" = @Id";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void GetClaimsByUserIdQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<GetClaimsByUserIdQuery>();
            var expected = "SELECT \"ClaimType\", \"ClaimValue\" FROM \"dbo\".\"IdentityUserClaim\" WHERE \"UserId\" = @UserId";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void GetRolesByUserIdQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<GetRolesByUserIdQuery>();
            var expected = "SELECT \"Name\" FROM \"dbo\".\"IdentityRole\", \"dbo\".\"IdentityUserRole\" WHERE \"dbo\".\"IdentityRole\".\"Id\" = \"dbo\".\"IdentityUserRole\".\"RoleId\" AND \"UserId\" = @UserId";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void GetUserLoginByLoginProviderAndProviderKeyQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<GetUserLoginByLoginProviderAndProviderKeyQuery, DapperIdentityUser>(new DapperIdentityUser());
            var expected = "SELECT \"IdentityUser\".\"AccessFailedCount\", \"IdentityUser\".\"Email\", \"IdentityUser\".\"EmailConfirmed\", \"IdentityUser\".\"LockoutEnabled\", \"IdentityUser\".\"LockoutEnd\", \"IdentityUser\".\"PasswordHash\", \"IdentityUser\".\"PhoneNumber\", \"IdentityUser\".\"PhoneNumberConfirmed\", \"IdentityUser\".\"SecurityStamp\", \"IdentityUser\".\"TwoFactorEnabled\", \"IdentityUser\".\"UserName\" FROM \"dbo\".\"IdentityUser\", \"dbo\".\"IdentityLogin\" WHERE \"dbo\".\"IdentityUser\".\"Id\" = \"dbo\".\"IdentityLogin\".\"UserId\" AND \"LoginProvider\" = @LoginProvider AND \"ProviderKey\" = @ProviderKey LIMIT 1";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void GetUserLoginByLoginProviderAndProviderKeyQueryGeneratesCorrectQueryWhenUsingCustomUser()
        {
            var generatedQuery = _queryFactory.GetQuery<GetUserLoginByLoginProviderAndProviderKeyQuery, CustomDapperIdentityUser>(new CustomDapperIdentityUser());
            var expected = "SELECT \"IdentityUser\".\"AccessFailedCount\", \"IdentityUser\".\"Dummy\", \"IdentityUser\".\"Email\", \"IdentityUser\".\"EmailConfirmed\", \"IdentityUser\".\"LockoutEnabled\", \"IdentityUser\".\"LockoutEnd\", \"IdentityUser\".\"PasswordHash\", \"IdentityUser\".\"PhoneNumber\", \"IdentityUser\".\"PhoneNumberConfirmed\", \"IdentityUser\".\"SecurityStamp\", \"IdentityUser\".\"TwoFactorEnabled\", \"IdentityUser\".\"UserName\" FROM \"dbo\".\"IdentityUser\", \"dbo\".\"IdentityLogin\" WHERE \"dbo\".\"IdentityUser\".\"Id\" = \"dbo\".\"IdentityLogin\".\"UserId\" AND \"LoginProvider\" = @LoginProvider AND \"ProviderKey\" = @ProviderKey LIMIT 1";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void GetUserLoginInfoByIdQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<GetUserLoginInfoByIdQuery>();
            var expected = "SELECT \"LoginProvider\", \"Name\", \"ProviderKey\" FROM \"dbo\".\"IdentityLogin\" WHERE \"UserId\" = @UserId";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void GetUsersByClaimQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<GetUsersByClaimQuery, DapperIdentityUser>(new DapperIdentityUser());
            var expected = "SELECT \"IdentityUser\".\"AccessFailedCount\", \"IdentityUser\".\"Email\", \"IdentityUser\".\"EmailConfirmed\", \"IdentityUser\".\"LockoutEnabled\", \"IdentityUser\".\"LockoutEnd\", \"IdentityUser\".\"PasswordHash\", \"IdentityUser\".\"PhoneNumber\", \"IdentityUser\".\"PhoneNumberConfirmed\", \"IdentityUser\".\"SecurityStamp\", \"IdentityUser\".\"TwoFactorEnabled\", \"IdentityUser\".\"UserName\" FROM \"dbo\".\"IdentityUser\", \"dbo\".\"IdentityUserClaim\" WHERE \"ClaimValue\" = @ClaimValue AND \"ClaimType\" = @ClaimType";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void GetUsersByClaimQueryGeneratesCorrectQueryWhenUsingCustomUser()
        {
            var generatedQuery = _queryFactory.GetQuery<GetUsersByClaimQuery, CustomDapperIdentityUser>(new CustomDapperIdentityUser());
            var expected = "SELECT \"IdentityUser\".\"AccessFailedCount\", \"IdentityUser\".\"Dummy\", \"IdentityUser\".\"Email\", \"IdentityUser\".\"EmailConfirmed\", \"IdentityUser\".\"LockoutEnabled\", \"IdentityUser\".\"LockoutEnd\", \"IdentityUser\".\"PasswordHash\", \"IdentityUser\".\"PhoneNumber\", \"IdentityUser\".\"PhoneNumberConfirmed\", \"IdentityUser\".\"SecurityStamp\", \"IdentityUser\".\"TwoFactorEnabled\", \"IdentityUser\".\"UserName\" FROM \"dbo\".\"IdentityUser\", \"dbo\".\"IdentityUserClaim\" WHERE \"ClaimValue\" = @ClaimValue AND \"ClaimType\" = @ClaimType";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void GetUsersInRoleQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<GetUsersInRoleQuery, DapperIdentityUser>(new DapperIdentityUser());
            var expected = "SELECT \"IdentityUser\".\"AccessFailedCount\", \"IdentityUser\".\"Email\", \"IdentityUser\".\"EmailConfirmed\", \"IdentityUser\".\"LockoutEnabled\", \"IdentityUser\".\"LockoutEnd\", \"IdentityUser\".\"PasswordHash\", \"IdentityUser\".\"PhoneNumber\", \"IdentityUser\".\"PhoneNumberConfirmed\", \"IdentityUser\".\"SecurityStamp\", \"IdentityUser\".\"TwoFactorEnabled\", \"IdentityUser\".\"UserName\" FROM \"dbo\".\"IdentityUser\", \"dbo\".\"IdentityUserRole\", \"dbo\".\"IdentityRole\" WHERE UPPER(\"dbo\".\"IdentityRole\".\"Name\") = @RoleName AND \"dbo\".\"IdentityUserRole\".\"RoleId\" = \"dbo\".\"IdentityRole\".\"Id\" AND \"dbo\".\"IdentityUserRole\".\"UserId\" = \"dbo\".\"IdentityUser\".\"Id\"";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void GetUsersInRoleQueryGeneratesCorrectQueryWhenUsingCustomUser()
        {
            var generatedQuery = _queryFactory.GetQuery<GetUsersInRoleQuery, CustomDapperIdentityUser>(new CustomDapperIdentityUser());
            var expected = "SELECT \"IdentityUser\".\"AccessFailedCount\", \"IdentityUser\".\"Dummy\", \"IdentityUser\".\"Email\", \"IdentityUser\".\"EmailConfirmed\", \"IdentityUser\".\"LockoutEnabled\", \"IdentityUser\".\"LockoutEnd\", \"IdentityUser\".\"PasswordHash\", \"IdentityUser\".\"PhoneNumber\", \"IdentityUser\".\"PhoneNumberConfirmed\", \"IdentityUser\".\"SecurityStamp\", \"IdentityUser\".\"TwoFactorEnabled\", \"IdentityUser\".\"UserName\" FROM \"dbo\".\"IdentityUser\", \"dbo\".\"IdentityUserRole\", \"dbo\".\"IdentityRole\" WHERE UPPER(\"dbo\".\"IdentityRole\".\"Name\") = @RoleName AND \"dbo\".\"IdentityUserRole\".\"RoleId\" = \"dbo\".\"IdentityRole\".\"Id\" AND \"dbo\".\"IdentityUserRole\".\"UserId\" = \"dbo\".\"IdentityUser\".\"Id\"";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void InsertUserClaimQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetInsertQuery<InsertUserClaimQuery, DapperIdentityUserClaim<int>>(new DapperIdentityUserClaim<int>());

            var expected = "INSERT INTO \"dbo\".\"IdentityUserClaim\" (\"ClaimType\", \"ClaimValue\", \"UserId\") VALUES(@ClaimType, @ClaimValue, @UserId)";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void InsertUserClaimQueryGeneratesCorrectQueryWhenProvidingId()
        {
            var generatedQuery = _queryFactory.GetInsertQuery<InsertUserClaimQuery, DapperIdentityUserClaim<int>>(new DapperIdentityUserClaim<int> { Id = 123 });

            var expected = "INSERT INTO \"dbo\".\"IdentityUserClaim\" (\"ClaimType\", \"ClaimValue\", \"Id\", \"UserId\") VALUES(@ClaimType, @ClaimValue, @Id, @UserId)";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void InsertUserLoginQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetInsertQuery<InsertUserLoginQuery, DapperIdentityUserLogin<int>>(new DapperIdentityUserLogin<int>());

            var expected = "INSERT INTO \"dbo\".\"IdentityLogin\" (\"LoginProvider\", \"ProviderDisplayName\", \"ProviderKey\", \"UserId\") VALUES(@LoginProvider, @ProviderDisplayName, @ProviderKey, @UserId)";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void InsertUserQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetInsertQuery<InsertUserQuery, DapperIdentityUser>(new DapperIdentityUser());

            var expected = "INSERT INTO \"dbo\".\"IdentityUser\" (\"AccessFailedCount\", \"Email\", \"EmailConfirmed\", \"LockoutEnabled\", \"LockoutEnd\", \"PasswordHash\", \"PhoneNumber\", \"PhoneNumberConfirmed\", \"SecurityStamp\", \"TwoFactorEnabled\", \"UserName\") VALUES(@AccessFailedCount, @Email, @EmailConfirmed, @LockoutEnabled, @LockoutEnd, @PasswordHash, @PhoneNumber, @PhoneNumberConfirmed, @SecurityStamp, @TwoFactorEnabled, @UserName) RETURNING \"Id\"";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void InsertUserQueryGeneratesCorrectQueryWhenUsingCustomUser()
        {
            var generatedQuery = _queryFactory.GetInsertQuery<InsertUserQuery, CustomDapperIdentityUser>(new CustomDapperIdentityUser());

            var expected = "INSERT INTO \"dbo\".\"IdentityUser\" (\"AccessFailedCount\", \"Dummy\", \"Email\", \"EmailConfirmed\", \"LockoutEnabled\", \"LockoutEnd\", \"PasswordHash\", \"PhoneNumber\", \"PhoneNumberConfirmed\", \"SecurityStamp\", \"TwoFactorEnabled\", \"UserName\") VALUES(@AccessFailedCount, @Dummy, @Email, @EmailConfirmed, @LockoutEnabled, @LockoutEnd, @PasswordHash, @PhoneNumber, @PhoneNumberConfirmed, @SecurityStamp, @TwoFactorEnabled, @UserName) RETURNING \"Id\"";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void InsertUserRoleQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetInsertQuery<InsertUserRoleQuery, DapperIdentityUserRole<int>>(new DapperIdentityUserRole<int>());

            var expected = "INSERT INTO \"dbo\".\"IdentityUserRole\" (\"RoleId\", \"UserId\") VALUES(@RoleId, @UserId)";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void IsInRoleQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<IsInRoleQuery, DapperIdentityUser>(new DapperIdentityUser());
            var expected = "SELECT 1 FROM \"dbo\".\"IdentityUser\", \"dbo\".\"IdentityUserRole\", \"dbo\".\"IdentityRole\" WHERE UPPER(\"dbo\".\"IdentityRole\".\"Name\") = @RoleName AND \"dbo\".\"IdentityUser\".\"Id\" = @UserId AND \"dbo\".\"IdentityUserRole\".\"RoleId\" = \"dbo\".\"IdentityRole\".\"Id\" AND \"dbo\".\"IdentityUserRole\".\"UserId\" = \"dbo\".\"IdentityUser\".\"Id\"";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void RemoveClaimsQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetDeleteQuery<RemoveClaimsQuery>();

            var expected = "DELETE FROM \"dbo\".\"IdentityUserClaim\" WHERE \"UserId\" = @UserId AND \"ClaimType\" = @ClaimType AND \"ClaimValue\" = @ClaimValue";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void RemoveLoginForUserQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetDeleteQuery<RemoveLoginForUserQuery>();

            var expected = "DELETE FROM \"dbo\".\"IdentityLogin\" WHERE \"UserId\" = @UserId AND \"LoginProvider\" = @LoginProvider AND \"ProviderKey\" = @ProviderKey";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void RemoveUserFromRoleQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetDeleteQuery<RemoveUserFromRoleQuery>();
            var expected = "DELETE FROM \"dbo\".\"IdentityUserRole\" USING \"dbo\".\"IdentityRole\" WHERE \"dbo\".\"IdentityUserRole\".\"RoleId\" = \"dbo\".\"IdentityRole\".\"Id\" AND \"dbo\".\"IdentityUserRole\".\"UserId\" = @UserId AND UPPER(\"dbo\".\"IdentityRole\".\"Name\") = @RoleName";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void SelectUserByEmailQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<SelectUserByEmailQuery>();

            var expected = "SELECT * FROM \"dbo\".\"IdentityUser\" WHERE UPPER(\"Email\") = @Email";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void SelectUserByIdQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<SelectUserByIdQuery>();

            var expected = "SELECT * FROM \"dbo\".\"IdentityUser\" WHERE \"Id\" = @Id";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void SelectUserByUserNameQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<SelectUserByUserNameQuery>();

            var expected = "SELECT * FROM \"dbo\".\"IdentityUser\" WHERE UPPER(\"UserName\") = @UserName";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void UpdateClaimForUserQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetUpdateQuery<UpdateClaimForUserQuery, DapperIdentityUserClaim<int>>(new DapperIdentityUserClaim<int>());

            var expected = "UPDATE \"dbo\".\"IdentityUserClaim\" SET \"ClaimType\" = @NewClaimType, \"ClaimValue\" = @NewClaimValue WHERE \"UserId\" = @UserId AND \"ClaimType\" = @ClaimType AND \"ClaimValue\" = @ClaimValue";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void UpdateUserQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetUpdateQuery<UpdateUserQuery, DapperIdentityUser>(new DapperIdentityUser());

            var expected = "UPDATE \"dbo\".\"IdentityUser\" SET \"AccessFailedCount\" = @AccessFailedCount, \"Email\" = @Email, \"EmailConfirmed\" = @EmailConfirmed, \"LockoutEnabled\" = @LockoutEnabled, \"LockoutEnd\" = @LockoutEnd, \"PasswordHash\" = @PasswordHash, \"PhoneNumber\" = @PhoneNumber, \"PhoneNumberConfirmed\" = @PhoneNumberConfirmed, \"SecurityStamp\" = @SecurityStamp, \"TwoFactorEnabled\" = @TwoFactorEnabled, \"UserName\" = @UserName WHERE \"Id\" = @Id";

            Assert.Equal(expected, generatedQuery);
        }
    }
}
