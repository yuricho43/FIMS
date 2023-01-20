using System;
using System.Net.Http;
//using Microsoft.AspNetCore.Components.WebAssembly.Hosting; //JBH: should not use for MauiBlazor!!
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using Telerik.Blazor.Services;

//using Blazored.LocalStorage;

//using Fims.Client.MauiBlazor.Data;

using Fims.Client.Shared.Infrastructure; //for ApiAuthenticationStateProvider
using Fims.Client.Shared.ClientServices.Authentication;
using Fims.Client.Shared.ClientServices.TSheets;
using Fims.Client.Shared.ClientServices.TSheetSpecs;

using Fims.Client.Shared.Localization;
using Fims.Client.Shared.Shared.Layouts;

using Fims.Client.MauiBlazor.Infrastructure.HttpDev;
using Fims.Client.Shared.ClientServices.TSheetSpecsInProgress;

namespace Fims.Client.MauiBlazor
{
    public static class MauiProgram
    {
#if JBH_USE_ORIGINAL
       public static string Base = DeviceInfo.Platform == DevicePlatform.Android ? "https://10.0.2.2" : "https://localhost";
        public static string BaseAddress = $"{Base}:5001/";
#endif

        private const string ClientName = "Fims.ServerAPI";

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
            builder.Services.AddHttpClient(
                    ClientName,
                    //JBH client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                    client => client.BaseAddress = new Uri(BaseAddress)
                )
                .AddHttpMessageHandler<AuthenticationHeaderHandler>();
#else
#if DEBUG
            builder.Services.AddTransient<AuthenticationHeaderHandler>();
            builder.Services.AddLocalDevHttpClient(ClientName, 5001);
#else
            builder.Services.AddTransient<AuthenticationHeaderHandler>();
            builder.Services.AddHttpClient(ClientName, client =>
            {
                client.BaseAddress = new Uri($"https://{LocalDevHttpClientHelper.DevServerName}:5001");
            });
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