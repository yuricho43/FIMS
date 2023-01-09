using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

//using Blazored.LocalStorage;


namespace Fims.Client.Shared.Infrastructure
{
    public class AuthenticationHeaderHandler : DelegatingHandler
    {
        private readonly LocalStorageInterop localStorage; //JBH: changed from ILocalStorageService

        public AuthenticationHeaderHandler(LocalStorageInterop localStorage) //JBH: changed from ILocalStorageService
            => this.localStorage = localStorage;

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (request.Headers.Authorization?.Scheme != "Bearer")
            {
                /////////////////////////////////////////////////////////////////////////////////////
                //JBH SEE: https://github.com/dotnet/maui/issues/2547
                //JBH FIXME
                //  _jsRuntime.InvokeAsync
                //      - OK in the genuine Web Browser
                //      - Exception in WebView
                //
                //  Exception from blazor.webview.js
                //  Cannot invoke JavaScript outside of a WebView context.
                //
                //  How to fix?
                //     Use JSInterop after the app was rendered, when JSRuntime is available.
                //     See @enetstudio comments: https://github.com/dotnet/maui/issues/2547
                /////////////////////////////////////////////////////////////////////////////////////
                ///

                string savedToken = string.Empty;
                if ( !request.RequestUri.ToString().Contains("/identity/login")) //do not put authToken for the login request
                {
                    savedToken = await this.localStorage.GetItem<string>("authToken");
                }

                if (!string.IsNullOrWhiteSpace(savedToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
