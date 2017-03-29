using Identity.Dapper.Entities;
using Identity.Dapper.Models;
using Identity.Dapper.MySQL;
using Identity.Dapper.PostgreSQL;
using Identity.Dapper.Samples.Web.Entities;
using Identity.Dapper.Samples.Web.Services;
using Identity.Dapper.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Identity.Dapper.Samples.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureDapperSqlServerConnectionProvider(Configuration.GetSection("DapperIdentity"))
                    .ConfigureDapperIdentityCryptography(Configuration.GetSection("DapperIdentityCryptography"))
                    .ConfigureDapperIdentityOptions(new DapperIdentityOptions { UseTransactionalBehavior = false });

            //services.ConfigureDapperPostgreSqlConnectionProvider(Configuration.GetSection("DapperIdentity"))
            //        .ConfigureDapperIdentityCryptography(Configuration.GetSection("DapperIdentityCryptography"));

            //services.ConfigureDapperMySqlConnectionProvider(Configuration.GetSection("DapperIdentity"))
            //        .ConfigureDapperIdentityCryptography(Configuration.GetSection("DapperIdentityCryptography"));

            services.AddIdentity<CustomUser, CustomRole>(x =>
                                                         {
                                                             x.Password.RequireDigit = false;
                                                             x.Password.RequiredLength = 1;
                                                             x.Password.RequireLowercase = false;
                                                             x.Password.RequireNonAlphanumeric = false;
                                                             x.Password.RequireUppercase = false;
                                                         })
                    //.AddDapperIdentityForPostgreSql()
                    .AddDapperIdentityForSqlServer()
                    //.AddDapperIdentityForMySql()
                    .AddDefaultTokenProviders();

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
