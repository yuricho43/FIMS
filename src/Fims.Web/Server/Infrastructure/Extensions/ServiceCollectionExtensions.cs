using System;
using System.Linq;
using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;

using Fims.Services.Common;
using Fims.Data;
using Fims.Data.Contracts;
using Fims.Data.Entities;
//using Fims.Data.Seed;
using Fims.Data.Models;

using static Fims.Data.ModelConstants.Identity;
using Fims.Web.Server.Infrastructure.Filters;
using Fims.Web.Server.Infrastructure.Services;
using System.Text.Json.Serialization;

namespace Fims.Web.Server.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static ApplicationSettings GetApplicationSettings(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var applicationSettingsConfiguration = configuration.GetSection(nameof(ApplicationSettings));
            services.Configure<ApplicationSettings>(applicationSettingsConfiguration);
            return applicationSettingsConfiguration.Get<ApplicationSettings>();
            /*
             * get "ApplicationSettings" section out of appsettings.json, such as "Secret": "S0M3RAN0MS3CR3T!1!MAG1C!1!"
             *    "ApplicationSettings": {
             *        "Secret": "S0M3RAN0MS3CR3T!1!MAG1C!1!"
             *    },
             *    "ConnectionStrings": {
             *                  "DefaultConnection": "Server=.\\SQLEXPRESS;Database=FimsDb;Trusted_Connection=True;MultipleActiveResultSets=true"
             *    },
             */

        }

        public static IServiceCollection AddDatabase(
            this IServiceCollection services,
            IConfiguration configuration)
            => services
                .AddDbContext<FimsDbContext>(options => options
                 //JBH    .UseSqlServer(configuration.GetDefaultConnectionString(),
                 //JBH                  x => x.MigrationsAssembly("Fims.Data")))
                    .UseSqlServer(configuration.GetDefaultConnectionString())
                    .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information)
                    .EnableSensitiveDataLogging()
                )
                //.AddTransient<IInitialData, CategoriesData>()
                //.AddTransient<IInitialData, ProductsData>()
                .AddTransient<IInitializer, FimsDbInitializer>();
                /*
                 * create and register instances to the DI container "services" (IServiceCollection)
                 * For example, create a FimsDbContext instance, and register it to the DI container.
                 * later on, any class constructor which needs this FimsDbContext instance will be
                 * given/injected the instance automatically by the DI container.
                 * 
                 * Also note that we're registering some "data instances" IInitialData instances (CategoriesData instance & ProductsData instance),
                 * which will be used as the initial "seeding" (Fims.Data.Seed folder)
                 * These "data instances" will be instantiated/injected automatically by DI to where necessary.
                 */

        public static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services
                .AddIdentity<FimsUser, FimsRole>(options =>
                {
                    //JBH: data type of "options" --> Microsoft.AspNetCore.Identity.IdentityOptions.
                    //     By setting options here, we can configure the MS Identity for my purpose.
                    options.Password.RequiredLength = MinPasswordLength;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<FimsDbContext>();

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            ApplicationSettings applicationSettings)
        {
            var key = Encoding.ASCII.GetBytes(applicationSettings.Secret); //Secret comes from the "ApplicationSettings" section @ appsettings.json

            services
                .AddAuthentication(authentication =>
                {
                    //JBH: add/register the Identity authentication to the DI container (IServiceCollection)
                    //     configure the Identity authentication options, so that we'll use the JwtBearer authentication scheme.
                    //     The default was the "Identity.Application" scheme,
                    //     which will be configured/changed/set to "Bearer" scheme.
                    authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(bearer =>
                {
                    //JBH: Since we've configured to the JwtBearer authentication scheme above,
                    //     add/register the JwtBearer handler to DI.
                    bearer.RequireHttpsMetadata = false;
                    bearer.SaveToken = true;
                    bearer.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>(); //CurrentUserService gets the current user from HttpContext

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            //JBH: add/register all my application services to the DI container.
            //     "all my application services" --> all services under the directory "Fims.Services",
            //     such as "ProductService", "ShoppingCartService", "IdentityService", ...
            var serviceInterfaceType = typeof(IService);
            var singletonServiceInterfaceType = typeof(ISingletonService);
            var scopedServiceInterfaceType = typeof(IScopedService);

            var types = serviceInterfaceType
                .Assembly
                .GetExportedTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .Select(t => new
                {
                    Service = t.GetInterface($"I{t.Name}"),
                    Implementation = t
                })
                .Where(t => t.Service != null);

            foreach (var type in types)
            {
                if (serviceInterfaceType.IsAssignableFrom(type.Service))
                {
                    services.AddTransient(type.Service, type.Implementation);
                }
                else if (singletonServiceInterfaceType.IsAssignableFrom(type.Service))
                {
                    services.AddSingleton(type.Service, type.Implementation);
                }
                else if (scopedServiceInterfaceType.IsAssignableFrom(type.Service))
                {
                    services.AddScoped(type.Service, type.Implementation);
                }
            }

            return services;
        }

        public static IServiceCollection AddApiControllers(this IServiceCollection services)
        {
            // services
            //     .AddControllers(options => options
            //           .Filters
            //           .Add<ModelOrNotFoundActionFilter>());

            //JBH: To fix an error: 
            //     A possible object cycle was detected.
            //     This can either be due to a cycle or if the object depth is larger than the maximum allowed depth of 32.
            //     Consider using ReferenceHandler.Preserve on JsonSerializerOptions to support cycles.
            services
                .AddControllers(options => options.Filters.Add<ModelOrNotFoundActionFilter>()).AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.WriteIndented = true;
                });

            services.AddRazorPages();

            return services;
        }
    }
}
