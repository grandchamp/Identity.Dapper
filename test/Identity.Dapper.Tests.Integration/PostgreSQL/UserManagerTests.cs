using Identity.Dapper.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Identity.Dapper.Tests.Integration.PostgreSQL
{
    //TODO:
    //There's a little problem with IClassFixture that on EVERY test, the constructor of the class is called (and if implements IDisposable, the Dispose() is called too)
    //So, there's no safe way to clean data of the database.
    //As a workaround, every time you run this test, execute restart.sh to reset all data on Docker container
    [Collection(nameof(PostgreSQL))]
    [TestCaseOrderer(TestCollectionOrderer.TypeName, TestCollectionOrderer.AssemblyName)]
    public class UserManagerTestsPostgreSql : IClassFixture<PostgreDatabaseFixture>
    {
        private readonly PostgreDatabaseFixture _databaseFixture;
        private readonly UserManager<DapperIdentityUser> _userManager;
        public UserManagerTestsPostgreSql(PostgreDatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
            _userManager = (UserManager<DapperIdentityUser>)_databaseFixture.TestServer.Host.Services.GetService(typeof(UserManager<DapperIdentityUser>));
        }

        [Fact, TestPriority(200)]
        public async Task CanCreateUserWithoutPassword()
        {
            var result = await _userManager.CreateAsync(new DapperIdentityUser
            {
                UserName = "test",
                Email = "test@test.com"
            });

            Assert.True(result.Succeeded);
        }

        [Fact, TestPriority(201)]
        public async Task CanCreateUserWithPassword()
        {
            var result = await _userManager.CreateAsync(new DapperIdentityUser
            {
                UserName = "test2",
                Email = "test2@test2.com"
            }, "123456");

            Assert.True(result.Succeeded);
        }

        [Fact, TestPriority(202)]
        public async Task CantCreateDuplicateUser()
        {
            var result = await _userManager.CreateAsync(new DapperIdentityUser
            {
                UserName = "test",
                Email = "test@test.com"
            });

            Assert.False(result.Succeeded);
            Assert.Contains(result.Errors, x => x.Code.Equals(new IdentityErrorDescriber().DuplicateUserName("").Code));
        }

        [Fact, TestPriority(203)]
        public async Task CanFindUserByName()
        {
            var result = await _userManager.FindByNameAsync("test");

            Assert.NotNull(result);
        }

        [Fact, TestPriority(204)]
        public async Task CanCreateUserWithEmptyUserName()
        {
            var result = await _userManager.CreateAsync(new DapperIdentityUser { Email = "test@test.com" });

            Assert.Contains(result.Errors, x => x.Code.Equals(new IdentityErrorDescriber().InvalidUserName("").Code));
            Assert.False(result.Succeeded);
        }

        [Fact, TestPriority(205)]
        public async Task CanIncreaseAccessFailedCount()
        {
            var user = await _userManager.FindByNameAsync("test");
            var result = await _userManager.AccessFailedAsync(user);

            Assert.True(result.Succeeded);
        }

        [Fact, TestPriority(206)]
        public async Task CanGetAccessFailedCount()
        {
            var user = await _userManager.FindByNameAsync("test");
            var result = await _userManager.GetAccessFailedCountAsync(user);

            Assert.True(result > 0);
        }

        [Fact, TestPriority(207)]
        public async Task CanAddClaim()
        {
            await _userManager.CreateAsync(new DapperIdentityUser { UserName = "claim", Email = "claim@claim.com" }, "123456");

            var user = await _userManager.FindByNameAsync("claim");
            Assert.NotNull(user);

            var result = await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Actor, "test"));
            Assert.True(result.Succeeded);
        }

        [Fact, TestPriority(208)]
        public async Task CanAddClaims()
        {
            var user = await _userManager.FindByNameAsync("claim");
            Assert.NotNull(user);

            var claim1 = new Claim(ClaimTypes.AuthenticationMethod, "test2");
            var claim2 = new Claim(ClaimTypes.AuthorizationDecision, "test3");

            var result = await _userManager.AddClaimsAsync(user, new[] { claim1, claim2 });
            Assert.True(result.Succeeded);
        }

        [Fact, TestPriority(209)]
        public async Task CanGetClaims()
        {
            var user = await _userManager.FindByNameAsync("claim");
            Assert.NotNull(user);

            var result = await _userManager.GetClaimsAsync(user);

            Assert.Collection(result, x => x.Type.Equals(ClaimTypes.AuthenticationMethod),
                                      x => x.Type.Equals(ClaimTypes.AuthorizationDecision),
                                      x => x.Type.Equals(ClaimTypes.Actor));
        }

        [Fact, TestPriority(210)]
        public async Task CanRemoveClaim()
        {
            var user = await _userManager.FindByNameAsync("claim");
            Assert.NotNull(user);

            var result = await _userManager.RemoveClaimAsync(user, new Claim(ClaimTypes.Actor, "test"));

            Assert.True(result.Succeeded);
        }

        [Fact, TestPriority(211)]
        public async Task CanRemoveClaims()
        {
            var user = await _userManager.FindByNameAsync("claim");
            Assert.NotNull(user);

            var claim1 = new Claim(ClaimTypes.AuthenticationMethod, "test2");
            var claim2 = new Claim(ClaimTypes.AuthorizationDecision, "test3");

            var result = await _userManager.RemoveClaimsAsync(user, new[] { claim1, claim2 });

            Assert.True(result.Succeeded);
        }

        [Fact, TestPriority(212)]
        public async Task CanAddLogin()
        {
            await _userManager.CreateAsync(new DapperIdentityUser { UserName = "login", Email = "login@login.com" }, "123456");

            var user = await _userManager.FindByNameAsync("login");
            Assert.NotNull(user);

            var result = await _userManager.AddLoginAsync(user, new UserLoginInfo("dummy", "dummy", "dummy"));
            var result2 = await _userManager.AddLoginAsync(user, new UserLoginInfo("dummy2", "dummy2", "dummy2"));

            Assert.True(result.Succeeded);
            Assert.True(result2.Succeeded);
        }

        [Fact, TestPriority(213)]
        public async Task CanGetLogin()
        {
            var user = await _userManager.FindByNameAsync("login");
            Assert.NotNull(user);

            var result = await _userManager.GetLoginsAsync(user);

            Assert.Collection(result, x => x.LoginProvider.Equals("dummy"), x => x.LoginProvider.Equals("dummy2"));
        }

        [Fact, TestPriority(214)]
        public async Task CanRemoveLogin()
        {
            var user = await _userManager.FindByNameAsync("login");
            Assert.NotNull(user);

            var result = await _userManager.RemoveLoginAsync(user, "dummy", "dummy");

            Assert.True(result.Succeeded);
        }

        [Fact, TestPriority(215)]
        public async Task CanAddPassword()
        {
            await _userManager.CreateAsync(new DapperIdentityUser { UserName = "test3", Email = "test3@test3.com" });

            var user = await _userManager.FindByNameAsync("test3");

            var result = await _userManager.AddPasswordAsync(user, "123456");

            Assert.True(result.Succeeded);
        }

        [Fact, TestPriority(216)]
        public async Task CanChangeEmail()
        {
            var user = await _userManager.FindByNameAsync("test3");

            var token = await _userManager.GenerateChangeEmailTokenAsync(user, "test3changed@test3.com");

            Assert.NotNull(token);

            var result = await _userManager.ChangeEmailAsync(user, "test3changed@test3.com", token);

            Assert.True(result.Succeeded);

            user = await _userManager.FindByNameAsync("test3");

            Assert.Equal("test3changed@test3.com", user.Email, ignoreCase: true);
        }

        [Fact, TestPriority(217)]
        public async Task CanChangePassword()
        {
            var user = await _userManager.FindByNameAsync("test3");

            Assert.NotNull(user);

            var result = await _userManager.ChangePasswordAsync(user, "123456", "123456789");

            Assert.True(result.Succeeded);
        }

        [Fact, TestPriority(218)]
        public async Task CanChangePhoneNumber()
        {
            var user = await _userManager.FindByNameAsync("test3");

            Assert.NotNull(user);

            var token = await _userManager.GenerateChangePhoneNumberTokenAsync(user, "123");

            Assert.NotNull(token);

            var result = await _userManager.ChangePhoneNumberAsync(user, "123", token);

            Assert.True(result.Succeeded);

            user = await _userManager.FindByNameAsync("test3");

            Assert.Equal("123", user.PhoneNumber);
        }

        [Fact, TestPriority(219)]
        public async Task CanCheckPassword()
        {
            var user = await _userManager.FindByNameAsync("test3");

            var result = await _userManager.CheckPasswordAsync(user, "123456789");

            Assert.True(result);
        }

        [Fact, TestPriority(220)]
        public async Task CanVerifyEmail()
        {
            var user = await _userManager.FindByNameAsync("test3");

            Assert.NotNull(user);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            Assert.NotNull(token);

            var result = await _userManager.ConfirmEmailAsync(user, token);

            Assert.True(result.Succeeded);

            user = await _userManager.FindByNameAsync("test3");

            Assert.True(user.EmailConfirmed);
        }

        [Fact, TestPriority(221)]
        public async Task CanFindById()
        {
            var user = await _userManager.FindByIdAsync(2.ToString());

            Assert.NotNull(user);
        }

        [Fact, TestPriority(222)]
        public async Task CanFindByEmail()
        {
            var user = await _userManager.FindByEmailAsync("test3changed@test3.com");

            Assert.NotNull(user);
        }

        [Fact, TestPriority(223)]
        public async Task CanFindByLogin()
        {
            var user = await _userManager.FindByLoginAsync("dummy2", "dummy2");

            Assert.NotNull(user);
        }

        [Fact, TestPriority(224)]
        public async Task CanGetPhoneNumber()
        {
            var user = await _userManager.FindByNameAsync("test3");

            var result = await _userManager.GetPhoneNumberAsync(user);

            Assert.Equal("123", result);
        }

        [Fact, TestPriority(225)]
        public async Task CanUpdateClaim()
        {
            var user = await _userManager.FindByNameAsync("claim");
            Assert.NotNull(user);

            var claim1 = new Claim(ClaimTypes.Actor, "test");
            await _userManager.AddClaimAsync(user, claim1);

            var claim2 = new Claim(ClaimTypes.Actor, "test2");
            await _userManager.AddClaimAsync(user, claim2);

            var result = await _userManager.ReplaceClaimAsync(user, claim1, claim2);

            Assert.True(result.Succeeded);

            var claims = await _userManager.GetClaimsAsync(user);

            Assert.Collection(claims, x => x.Type.Equals(ClaimTypes.Actor), x => x.Value.Equals("test2"));
        }

        [Fact, TestPriority(226)]
        public async Task CanResetAccessFailedCount()
        {
            var user = await _userManager.FindByNameAsync("test");
            Assert.NotNull(user);

            var actualAccessCount = await _userManager.GetAccessFailedCountAsync(user);

            Assert.Equal(1, actualAccessCount);

            var result = await _userManager.ResetAccessFailedCountAsync(user);

            Assert.True(result.Succeeded);

            actualAccessCount = await _userManager.GetAccessFailedCountAsync(user);

            Assert.Equal(0, actualAccessCount);
        }

        [Fact, TestPriority(227)]
        public async Task CanUpdate()
        {
            var user = await _userManager.FindByNameAsync("test");
            Assert.NotNull(user);

            user.Email = "testchanged@test.com";

            var result = await _userManager.UpdateAsync(user);

            Assert.True(result.Succeeded);

            user = await _userManager.FindByNameAsync("test");

            Assert.Equal("testchanged@test.com", user.Email, ignoreCase: true);
        }

        [Fact, TestPriority(228)]
        public async Task CanRemoveUser()
        {
            var user = await _userManager.FindByNameAsync("test");

            var result = await _userManager.DeleteAsync(user);

            Assert.True(result.Succeeded);
        }
    }
}
