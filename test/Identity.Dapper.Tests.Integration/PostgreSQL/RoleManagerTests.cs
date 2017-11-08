using Identity.Dapper.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Identity.Dapper.Tests.Integration.PostgreSQL
{
    //[Collection("PostgreSQL")]
    //[TestCaseOrderer(TestCollectionOrderer.TypeName, TestCollectionOrderer.AssemblyName)]
    //public class RoleManagerTestsPostgreSql : IClassFixture<PostgreDatabaseFixture>
    //{
    //    private readonly PostgreDatabaseFixture _databaseFixture;
    //    private readonly RoleManager<DapperIdentityRole> _roleManager;
    //    private readonly UserManager<DapperIdentityUser> _userManager;

    //    public RoleManagerTestsPostgreSql(PostgreDatabaseFixture databaseFixture)
    //    {
    //        _databaseFixture = databaseFixture;
    //        _roleManager = (RoleManager<DapperIdentityRole>)_databaseFixture.TestServer.Host.Services.GetService(typeof(RoleManager<DapperIdentityRole>));
    //        _userManager = (UserManager<DapperIdentityUser>)_databaseFixture.TestServer.Host.Services.GetService(typeof(UserManager<DapperIdentityUser>));
    //    }

    //    [Fact, TestPriority(300)]
    //    public async Task CanCreate()
    //    {
    //        var result = await _roleManager.CreateAsync(new DapperIdentityRole { Name = "test" });
    //        var result2 = await _roleManager.CreateAsync(new DapperIdentityRole { Name = "test2" });
    //        var result3 = await _roleManager.CreateAsync(new DapperIdentityRole { Name = "test3" });

    //        Assert.True(result.Succeeded);
    //        Assert.True(result2.Succeeded);
    //        Assert.True(result3.Succeeded);
    //    }

    //    [Fact, TestPriority(301)]
    //    public async Task CanFindByName()
    //    {
    //        var role = await _roleManager.FindByNameAsync("test");

    //        Assert.NotNull(role);
    //    }

    //    [Fact, TestPriority(302)]
    //    public async Task CanFindById()
    //    {
    //        var role = await _roleManager.FindByIdAsync(1.ToString());

    //        Assert.NotNull(role);
    //    }

    //    [Fact, TestPriority(303)]
    //    public async Task CanRemove()
    //    {
    //        await _roleManager.CreateAsync(new DapperIdentityRole { Name = "test4" });

    //        var role = await _roleManager.FindByNameAsync("test4");

    //        Assert.NotNull(role);

    //        var result = await _roleManager.DeleteAsync(role);

    //        Assert.True(result.Succeeded);
    //    }

    //    [Fact, TestPriority(304)]
    //    public async Task CanRoleExists()
    //    {
    //        var result = await _roleManager.RoleExistsAsync("test");

    //        Assert.True(result);
    //    }

    //    [Fact, TestPriority(305)]
    //    public async Task CanUpdate()
    //    {
    //        var role = await _roleManager.FindByNameAsync("test");
    //        role.Name = "testmodified";

    //        var result = await _roleManager.UpdateAsync(role);

    //        Assert.True(result.Succeeded);
    //    }

    //    [Fact, TestPriority(306)]
    //    public async Task CanAddRoleToUser()
    //    {
    //        await _userManager.CreateAsync(new DapperIdentityUser { UserName = "testrole", Email = "test@test.com" }, "123456");

    //        var user = await _userManager.FindByNameAsync("testrole");

    //        var result = await _userManager.AddToRoleAsync(user, "testmodified");

    //        Assert.True(result.Succeeded);
    //    }

    //    [Fact, TestPriority(307)]
    //    public async Task CanAddRolesToUser()
    //    {
    //        var user = await _userManager.FindByNameAsync("testrole");

    //        var result = await _userManager.AddToRolesAsync(user, new[] { "test2", "test3" });

    //        Assert.True(result.Succeeded);
    //    }

    //    [Fact, TestPriority(308)]
    //    public async Task CanGetRoles()
    //    {
    //        var user = await _userManager.FindByNameAsync("testrole");

    //        var result = await _userManager.GetRolesAsync(user);

    //        Assert.Contains(result, x => x.Equals("TESTMODIFIED") || x.Equals("TEST2") || x.Equals("TEST3"));
    //    }

    //    [Fact, TestPriority(309)]
    //    public async Task CanGetUsersInRole()
    //    {
    //        var result = await _userManager.GetUsersInRoleAsync("testmodified");

    //        Assert.Collection(result, x => x.UserName.Equals("testrole"));
    //    }

    //    [Fact, TestPriority(310)]
    //    public async Task CanCheckIfUserIsInRole()
    //    {
    //        var user = await _userManager.FindByNameAsync("testrole");

    //        var result = await _userManager.IsInRoleAsync(user, "testmodified");

    //        Assert.True(result);
    //    }

    //    [Fact, TestPriority(311)]
    //    public async Task CanRemoveUserFromRole()
    //    {
    //        var user = await _userManager.FindByNameAsync("testrole");

    //        var result = await _userManager.RemoveFromRoleAsync(user, "testmodified");

    //        Assert.True(result.Succeeded);
    //    }

    //    [Fact, TestPriority(312)]
    //    public async Task CanRemoveUserFromRoles()
    //    {
    //        var user = await _userManager.FindByNameAsync("testrole");

    //        var result = await _userManager.RemoveFromRolesAsync(user, new[] { "test2", "test3" });

    //        Assert.True(result.Succeeded);
    //    }
    //}
}
