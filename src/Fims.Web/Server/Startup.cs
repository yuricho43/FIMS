using System.Reflection;

using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Fims.Web.Server.Infrastructure.Extensions;
using Fims.Web.Server.Middleware;
using Microsoft.Extensions.Hosting;
using Fims.Data.Models.TSheets;
using Fims.Services.TSheets;
using Fims.Services.TSheetSpecs;
using Fims.Services.TSheetSpecsInProgress;
using Fims.Services.TSheetSpecsInClose;
using Fims.Services.TReports;
using Microsoft.AspNetCore.Identity;
using Telerik.SvgIcons;
using Fims.Data.Entities;
using Fims.Data;
using System.IO;

namespace Fims.Web.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
            => this.Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var fimsTSheetSpecsRepoPath = Configuration.GetValue<string>("FimsRepositories:FimsTSheetSpecsRepository");
            if (!Directory.Exists(fimsTSheetSpecsRepoPath)) { Directory.CreateDirectory(fimsTSheetSpecsRepoPath); }

            var fimsTReportSpecsRepoPath = Configuration.GetValue<string>("FimsRepositories:FimsTReportSpecsRepository");
            if (!Directory.Exists(fimsTReportSpecsRepoPath)) { Directory.CreateDirectory(fimsTReportSpecsRepoPath); }

            var fimsTReportOutputRepoPath = Configuration.GetValue<string>("FimsRepositories:FimsTReportOutputRepository");
            if (!Directory.Exists(fimsTReportOutputRepoPath)) { Directory.CreateDirectory(fimsTReportOutputRepoPath); }

            var fimsTSheetSpecsInProgressRepoPath = Configuration.GetValue<string>("FimsRepositories:FimsTSheetSpecsInProgressRepository");
            if (!Directory.Exists(fimsTSheetSpecsInProgressRepoPath)) { Directory.CreateDirectory(fimsTSheetSpecsInProgressRepoPath); }

            var fimsTSheetSpecsInCloseRepoPath = Configuration.GetValue<string>("FimsRepositories:FimsTSheetSpecsInCloseRepository");
            if (!Directory.Exists(fimsTSheetSpecsInCloseRepoPath)) { Directory.CreateDirectory(fimsTSheetSpecsInCloseRepoPath); }

            //JBH: Instantiate and AddSingleton TSheetSpecsService here,
            //     so that BuildTSheetSpecsFromFiles() @ TSheetSpecsService will run immediately upon the server startup.
            ITSheetSpecsService tSheetSpecsService = new TSheetSpecsService(Configuration);
            services.AddSingleton(tSheetSpecsService);

            ITSheetSpecsInProgressService   tSheetSpecsInProgressService    = new TSheetSpecsInProgressService(Configuration);
            services.AddSingleton(tSheetSpecsInProgressService);

            ITSheetSpecsInCloseService      tSheetSpecsInCloseService       = new TSheetSpecsInCloseService(Configuration);
            services.AddSingleton(tSheetSpecsInCloseService);

            // NOTE: ITSheetsService and ITReportsService inherits "IService",
            //       So those will be registered in AddApplicationServices().

            services.AddDatabase(this.Configuration); //add/register a DbContext (FimsDbContext) and initial db datas (CategoriesData, ProductsData) and db initializer (FimsDbInitializer) to the DI container (IServiceCollection)

            //services.AddIdentity();
            services.AddIdentity<FimsUser, FimsRole>(opt =>
                    {
                        opt.Password.RequiredLength = 6;
                        opt.Password.RequireDigit = false;
                        opt.Password.RequireUppercase = false;
                        opt.Password.RequireLowercase = false;
                        opt.Password.RequireNonAlphanumeric = false;
                        //opt.User.RequireUniqueEmail = true;
                    })
                    .AddEntityFrameworkStores<FimsDbContext>()
                    .AddDefaultTokenProviders(); //required for UserManager.GeneratePasswordResetTokenAsync(user)

            services.AddJwtAuthentication(services.GetApplicationSettings(this.Configuration));

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddApplicationServices();  //add/register all my application services (under the directory "Fims.Services") to the DI container.

            services.AddApiControllers();       //call AddControllers() and  AddRazorPages()

            services.AddEndpointsApiExplorer(); // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddSwaggerGen();
            /*
             * this.Configuration.Providers	Count = 4
             *     [0]	{Microsoft.Extensions.Configuration.ChainedConfigurationProvider}
             *     [1]	{JsonConfigurationProvider for 'appsettings.json' (Optional)}
             *     [2]	{JsonConfigurationProvider for 'appsettings.Development.json' (Optional)}
             *     [3]	{EnvironmentVariablesConfigurationProvider Prefix: ''}
             */
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Configure the HTTP request pipeline.
 
            if (env.IsDevelopment()) //JBH
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseExceptionHandling(env);
            app.UseValidationExceptionHandler();
            //FIXME  app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints();

            app.Initialize();  //call FimsDbInitializer which initializes/fills my db with initial db datas (CategoriesData, ProductsData)
        }
    }
}
