using System;
using System.Net.Http;
//using Microsoft.AspNetCore.Components.WebAssembly.Hosting; //JBH: should not use for MauiBlazor!!
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using Telerik.Blazor.Services;

//using Blazored.LocalStorage;

//using Fims.Inspect.Data;

using Fims.Client.Shared.Infrastructure; //for ApiAuthenticationStateProvider
using Fims.Client.Shared.ClientServices.Authentication;
using Fims.Client.Shared.ClientServices.TSheets;
using Fims.Client.Shared.ClientServices.TSheetSpecs;

using Fims.Client.Shared.Localization;
using Fims.Client.Shared.Shared.Layouts;

using Fims.Inspect.Infrastructure.HttpDev;
using Fims.Client.Shared.ClientServices.TSheetSpecsInProgress;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Fims.Inspect
{
    public static class MauiProgram
    {
        private const string ClientName = " Fims.Inspect";

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    // fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("Segoe-Ui-Bold.ttf", "SegoeUiBold");
                    fonts.AddFont("Segoe-Ui-Regular.ttf", "SegoeUiRegular");
                    // fonts.AddFont("Segoe-Ui-Semibold.ttf", "SegoeUiSemibold");
                    // fonts.AddFont("Segoe-Ui-Semilight.ttf", "SegoeUiSemilight");
                });

            builder.Services.AddMauiBlazorWebView();

            builder.Services.AddTelerikBlazor();

            // For appsettings.json to be used as IConfiguration
            // JBH NOTE: make sure appsettings.json enrolled as "EmbeddedResource": On the file property, set [Build Action] to "Embedded resouce".
            var a = Assembly.GetExecutingAssembly();
            var b = typeof(Fims.Client.Shared.Pages.discovery_stub).Assembly;
            using var appsettings_stream = b.GetManifestResourceStream("Fims.Client.Shared.appsettings.json");
            var configuration = new ConfigurationBuilder()
                .AddJsonStream(appsettings_stream)
                .Build();
            builder.Configuration.AddConfiguration(configuration);
            var car = configuration.GetValue<string>("Fims.Web.Server:remoteServerUrl"); //debug
            var dar = configuration.GetSection("Fims.Web.Server:remoteServerUrl"); //debug
            var ear = configuration.GetValue<string>("Fims.Web.Server:environmentVariables:ASPNETCORE_ENVIRONMENT"); //debug


#if JBH_USE_PLATFORM_NATIVE_SERVICE
#if WINDOWS
            builder.Services.AddSingleton<SharedMauiLib.INativeAudioService, SharedMauiLib.Platforms.Windows.NativeAudioService>();
#elif ANDROID
            builder.Services.AddSingleton<SharedMauiLib.INativeAudioService, SharedMauiLib.Platforms.Android.NativeAudioService>();
#elif MACCATALYST
            builder.Services.AddSingleton<SharedMauiLib.INativeAudioService, SharedMauiLib.Platforms.MacCatalyst.NativeAudioService>();
#elif IOS
            builder.Services.AddSingleton<SharedMauiLib.INativeAudioService, SharedMauiLib.Platforms.iOS.NativeAudioService>();
#endif
#endif

#if DEBUG
            //JBH: To debug using the browser dev tools (something like F12 on a browser)
            //     CTRL+SHIFT+I (or F12) --> will open the MS Edge DevTools
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            //builder.Services.AddSingleton<WeatherForecastService>();
            builder.Services.AddAuthorizationCore();

            //builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddScoped<LocalStorageInterop>();

            builder.Services.AddScoped<ApiAuthenticationStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();

            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(ClientName));
            builder.Services.AddTransient<IAuthClientService, AuthClientService>();

            builder.Services.AddTransient<ITSheetsClientService, TSheetsClientService>();
            builder.Services.AddTransient<ITSheetSpecsClientService, TSheetSpecsClientService>();
            builder.Services.AddTransient<ITSheetSpecsInProgressClientService, TSheetSpecsInProgressClientService>();

#if JBH_USE_ORIGINAL
            builder.Services.AddTransient<AuthenticationHeaderHandler>();
            string serverUrl = DeviceInfo.Platform == DevicePlatform.Android ? configuration.GetValue<string>("Fims.Web.Server:localServerUrl_android") : configuration.GetValue<string>("Fims.Web.Server:localServerUrl_windows");
            builder.Services.AddHttpClient(
                    ClientName,
                    client => client.BaseAddress = new Uri(serverUrl)
                )
                .AddHttpMessageHandler<AuthenticationHeaderHandler>();
#else
    #if DEBUG
            builder.Services.AddTransient<AuthenticationHeaderHandler>();
            string serverUrl;
            string LocalOrRemote = configuration.GetValue<string>("Fims.Web.Server:LocalOrRemote");
            if (LocalOrRemote == "Local")
            {
                serverUrl = DeviceInfo.Platform == DevicePlatform.Android ? configuration.GetValue<string>("Fims.Web.Server:localServerUrl_android") : configuration.GetValue<string>("Fims.Web.Server:localServerUrl_windows");
            }
            else
            {
                serverUrl = configuration.GetValue<string>("Fims.Web.Server:remoteServerUrl");
            }
            builder.Services.AddLocalDevHttpClient(ClientName, serverUrl);
    #else
            builder.Services.AddTransient<AuthenticationHeaderHandler>();
            string serverUrl;
            string LocalOrRemote = configuration.GetValue<string>("Fims.Web.Server:LocalOrRemote");
            if (LocalOrRemote == "Local")
            {
                serverUrl = DeviceInfo.Platform == DevicePlatform.Android ? configuration.GetValue<string>("Fims.Web.Server:localServerUrl_android") : configuration.GetValue<string>("Fims.Web.Server:localServerUrl_windows");
            }
            else
            {
                serverUrl = configuration.GetValue<string>("Fims.Web.Server:remoteServerUrl");
            }
            builder.Services.AddHttpClient(
                    ClientName,
                    client => client.BaseAddress = new Uri(serverUrl)
                )
                .AddHttpMessageHandler<AuthenticationHeaderHandler>();
    #endif
#endif

            builder.Services.AddScoped<MainLayoutState>();
            // register a custom localizer for the Telerik components, after registering the Telerik services
            builder.Services.AddSingleton<ITelerikStringLocalizer, TelerikLocalizer>();
            builder.Services.AddLocalization();

            return builder.Build();
        }
    }
}