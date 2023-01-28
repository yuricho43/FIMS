using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

using Telerik.Blazor.Services;

//using Blazored.LocalStorage;

using Fims.Client.Shared.Infrastructure; //for ApiAuthenticationStateProvider
using Fims.Client.Shared.ClientServices.Authentication;
using Fims.Client.Shared.ClientServices.TSheets;
using Fims.Client.Shared.ClientServices.TSheetSpecs;

using Fims.Client.Shared.Localization;
using Fims.Client.Shared.ClientServices.TSheetSpecsInProgress;
using Fims.Web.Client.Layouts;
using Fims.Client.Shared.Shared.Layouts;

namespace Fims.Web.Client
{
    public class Program
    {
        private const string ClientName = "Fims.ServerAPI";

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.RootComponents.Add<App>("app");

            builder.Services.AddTelerikBlazor();

            //builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddScoped<LocalStorageInterop>();

            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<ApiAuthenticationStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();

            builder.Services.AddScoped(sp => sp
                .GetRequiredService<IHttpClientFactory>()
                .CreateClient(ClientName));

            builder.Services.AddTransient<IAuthClientService, AuthClientService>();
            builder.Services.AddTransient<ITSheetsClientService, TSheetsClientService>();
            builder.Services.AddTransient<ITSheetSpecsClientService, TSheetSpecsClientService>();
            builder.Services.AddTransient<ITSheetSpecsInProgressClientService, TSheetSpecsInProgressClientService>();

            builder.Services.AddTransient<AuthenticationHeaderHandler>();

            //var baseAddress = builder.HostEnvironment.BaseAddress; // comes from applicationUrl@launchSettings.json
            var baseAddress = "http://localhost:5000";
            builder.Services.AddHttpClient(
                                ClientName,
                                client => client.BaseAddress = new Uri(baseAddress))
                            .AddHttpMessageHandler<AuthenticationHeaderHandler>();

            builder.Services.AddScoped<MainLayoutState>();
            // register a custom localizer for the Telerik components, after registering the Telerik services
            builder.Services.AddSingleton<ITelerikStringLocalizer, TelerikLocalizer>();
            builder.Services.AddLocalization();


            await builder.Build().RunAsync();
        }
    }
}
