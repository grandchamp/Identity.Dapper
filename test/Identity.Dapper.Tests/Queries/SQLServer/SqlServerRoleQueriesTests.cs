using Identity.Dapper.Entities;
using Identity.Dapper.Factories;
using Identity.Dapper.Factories.Contracts;
using Identity.Dapper.Queries;
using Identity.Dapper.Queries.Role;
using Identity.Dapper.SqlServer.Models;
using Identity.Dapper.Tests.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Identity.Dapper.Tests.Queries.SQLServer
{
    public class SqlServerRoleQueriesTests
    {
        private readonly IQueryFactory _queryFactory;
        public SqlServerRoleQueriesTests()
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
        public void DeleteRoleQueryGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetDeleteQuery<DeleteRoleQuery>();
            const string expected = "DELETE FROM [dbo].[IdentityRole] WHERE [Id] = @Id";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void InsertRoleQueryWithoutIdGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetInsertQuery<InsertRoleQuery, CustomDapperIdentityRole>(new CustomDapperIdentityRole());
            const string expected = "INSERT INTO [dbo].[IdentityRole] ([Dummy], [Name]) VALUES(@Dummy, @Name)";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void InsertRoleQueryWithIdGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetInsertQuery<InsertRoleQuery, CustomDapperIdentityRole>(new CustomDapperIdentityRole { Id = 2 });
            const string expected = "INSERT INTO [dbo].[IdentityRole] ([Dummy], [Id], [Name]) VALUES(@Dummy, @Id, @Name)";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void SelectRoleByIdGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<SelectRoleByIdQuery>();
            const string expected = "SELECT * FROM [dbo].[IdentityRole] WHERE [Id] = @Id";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void SelectRoleByNameGeneratesCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetQuery<SelectRoleByNameQuery>();
            const string expected = "SELECT * FROM [dbo].[IdentityRole] WHERE [Name] = @Name";

            Assert.Equal(expected, generatedQuery);
        }

        [Fact]
        public void UpdateRoleQueryGenerateCorrectQuery()
        {
            var generatedQuery = _queryFactory.GetUpdateQuery<UpdateRoleQuery, CustomDapperIdentityRole>(new CustomDapperIdentityRole
            {
                Id = 1,
                Name = "Teste",
                Dummy = "dummy"
            });


            const string expected = "UPDATE [dbo].[IdentityRole] SET [Dummy] = @Dummy, [Name] = @Name WHERE [Id] = @Id";

            Assert.Equal(expected, generatedQuery);
        }
    }
}
