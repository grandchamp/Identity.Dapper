using Identity.Dapper.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Identity.Dapper.Tests.Integration.PostgreSQL
{
    //TODO:
    //There's a little problem with IClassFixture that on EVERY test, the constructor of the class is called (and if implements IDisposable, the Dispose() is called too)
    //So, there's no safe way to clean data of the database.
    //As a workaround, i'm putting a breakpoint on CanCreateUser() and calling _postgreDatabaseFixture.DeleteAllData() via Immediate Window to clear the tables.
    [TestCaseOrderer(TestCollectionOrderer.TypeName, TestCollectionOrderer.AssemblyName)]
    public class UserManagerTests : IClassFixture<PostgreDatabaseFixture>
    {
        private readonly PostgreDatabaseFixture _postgreDatabaseFixture;
        private readonly UserManager<DapperIdentityUser> _userManager;

        public UserManagerTests(PostgreDatabaseFixture postgreDatabaseFixture)
        {
            _postgreDatabaseFixture = postgreDatabaseFixture;
            _userManager = (UserManager<DapperIdentityUser>)_postgreDatabaseFixture.TestServer.Host.Services.GetService(typeof(UserManager<DapperIdentityUser>));
        }

        [Fact, TestPriority(1)]
        public async Task CanCreateUser()
        {
            var result = await _userManager.CreateAsync(new DapperIdentityUser
            {
                UserName = "test",
                Email = "test@test.com"
            });

            Assert.True(result.Succeeded);
        }

        [Fact, TestPriority(2)] 
        public async Task CanFindUserByName()
        {
            var result = await _userManager.FindByNameAsync("test");

            Assert.NotNull(result);
        }

        [Fact, TestPriority(3)]
        public async Task CanRemoveUser()
        {
            var user = await _userManager.FindByNameAsync("test");

            var result = await _userManager.DeleteAsync(user);

            Assert.True(result.Succeeded);
        }
    }
}
