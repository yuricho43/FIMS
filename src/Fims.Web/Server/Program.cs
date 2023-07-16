using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;


using Serilog;
using Fims.Web.Server.Infrastructure.Extensions;
using Fims.Web.Server.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Fims.Data.Entities;
using Fims.Data;
using Fims.Services.Common;
using Fims.Services.TSheetSpecs;
using Fims.Services.TSheetSpecsInClose;
using Fims.Services.TSheetSpecsInProgress;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .WriteTo.SerilogCustomSink()
        .Enrich.FromLogContext()
        .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);
logger.Information("FIMS server started");


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Add builder.Services to the container.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////


var fimsTSheetSpecsRepoPath = builder.Configuration.GetValue<string>("FimsRepositories:FimsTSheetSpecsRepository");
if (!Directory.Exists(fimsTSheetSpecsRepoPath)) { Directory.CreateDirectory(fimsTSheetSpecsRepoPath); }

var fimsTReportSpecsRepoPath = builder.Configuration.GetValue<string>("FimsRepositories:FimsTReportSpecsRepository");
if (!Directory.Exists(fimsTReportSpecsRepoPath)) { Directory.CreateDirectory(fimsTReportSpecsRepoPath); }

var fimsTReportOutputRepoPath = builder.Configuration.GetValue<string>("FimsRepositories:FimsTReportOutputRepository");
if (!Directory.Exists(fimsTReportOutputRepoPath)) { Directory.CreateDirectory(fimsTReportOutputRepoPath); }

var fimsTSheetSpecsInProgressRepoPath = builder.Configuration.GetValue<string>("FimsRepositories:FimsTSheetSpecsInProgressRepository");
if (!Directory.Exists(fimsTSheetSpecsInProgressRepoPath)) { Directory.CreateDirectory(fimsTSheetSpecsInProgressRepoPath); }

var fimsTSheetSpecsInCloseRepoPath = builder.Configuration.GetValue<string>("FimsRepositories:FimsTSheetSpecsInCloseRepository");
if (!Directory.Exists(fimsTSheetSpecsInCloseRepoPath)) { Directory.CreateDirectory(fimsTSheetSpecsInCloseRepoPath); }

//JBH: Instantiate and AddSingleton TSheetSpecsService here,
//     so that BuildTSheetSpecsFromFiles() @ TSheetSpecsService will run immediately upon the server startup.
ITSheetSpecsService tSheetSpecsService = new TSheetSpecsService(builder.Configuration, logger);
var result = tSheetSpecsService.BuildTSheetSpecsFromExcelSpecFile();
builder.Services.AddSingleton(tSheetSpecsService);

ITSheetSpecsInProgressService tSheetSpecsInProgressService = new TSheetSpecsInProgressService(builder.Configuration, logger);
builder.Services.AddSingleton(tSheetSpecsInProgressService);

ITSheetSpecsInCloseService tSheetSpecsInCloseService = new TSheetSpecsInCloseService(builder.Configuration, logger);
builder.Services.AddSingleton(tSheetSpecsInCloseService);

// NOTE: ITSheetsService and ITReportsService inherits "IService",
//       So those will be registered in AddApplicationServices().

builder.Services.AddDatabase(builder.Configuration); //add/register a DbContext (FimsDbContext) and initial db datas (CategoriesData, ProductsData) and db initializer (FimsDbInitializer) to the DI container (IServiceCollection)

//builder.Services.AddIdentity();
builder.Services.AddIdentity<FimsUser, FimsRole>(opt =>
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

builder.Services.AddJwtAuthentication(builder.Services.GetApplicationSettings(builder.Configuration));

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddApplicationServices();  //add/register all my application builder.Services (under the directory "Fims.Services") to the DI container.

builder.Services.AddApiControllers();       //call AddControllers() and  AddRazorPages()

builder.Services.AddEndpointsApiExplorer(); // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen();
/*
 * this.Configuration.Providers	Count = 4
 *     [0]	{Microsoft.Extensions.Configuration.ChainedConfigurationProvider}
 *     [1]	{JsonConfigurationProvider for 'appsettings.json' (Optional)}
 *     [2]	{JsonConfigurationProvider for 'appsettings.Development.json' (Optional)}
 *     [3]	{EnvironmentVariablesConfigurationProvider Prefix: ''}
 */


//  builder.Services.AddControllers();
//  // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//  builder.Services.AddEndpointsApiExplorer();
//  builder.Services.AddSwaggerGen();


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Configure the HTTP request pipeline.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
var app = builder.Build();

//  if (app.Environment.IsDevelopment())
//  {
//      app.UseSwagger();
//      app.UseSwaggerUI();
//  }
//  
//  app.UseHttpsRedirection();
//  app.UseMiddleware(typeof(SerilogExceptionHandlingMiddleware));
//  app.UseAuthorization();
//  
//  app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseExceptionHandling(env);
// //app.UseValidationExceptionHandler();
app.UseMiddleware(typeof(SerilogExceptionHandlingMiddleware));
//FIXME  app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints();


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Initialize
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
app.Initialize();  //call FimsDbInitializer which initializes/fills my db with initial db datas (CategoriesData, ProductsData)


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Run it
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
app.Run();