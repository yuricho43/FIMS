using System.Security.Cryptography;
using System.Text;
using Fims.Client.Shared.Infrastructure.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;


namespace Fims.Client.Shared.Shared.NavMenu
{
    public partial class LoginDisplay
    {
        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        public string UserName { get; set; }

        protected override async Task OnInitializedAsync()
        {
            //FIXME    // Accessing LocalStorage at this phase is not allowed. JSRuntime out of WebView.
            //FIXME    // So do it after rendering finished.
            //FIXME    var state = await this.AuthState.GetAuthenticationStateAsync();
            //FIXME    var user1 = state.User;
            //FIXME    var name1 = user1.GetHangulName();
            //FIXME    var authState = await AuthenticationStateTask;
            //FIXME    if (authState.User.Identity.IsAuthenticated)
            //FIXME    {
            //FIXME        UserName = authState.User.GetUserName();
            //FIXME    }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            /////////////////////////////////////////////////////////////////////////////////////////////
            /// All JavaScript tasks should be done HERE!
            /// DO NOT at OnInitializedAsync().
            /////////////////////////////////////////////////////////////////////////////////////////////

            // Accessing LocalStorage at the initializing phase is not allowed. JSRuntime out of WebView.
            // So do it here after rendering finished.
            if (firstRender)
            {
                var state = await this.AuthState.GetAuthenticationStateAsync();
                var user1 = state.User;
                var name1 = user1.GetHangulName();

                var authState = await AuthenticationStateTask;
                if (authState.User.Identity.IsAuthenticated)
                {
                    UserName = authState.User.GetUserName();
                }
                StateHasChanged();
            }
        }

        private void BeginSignOut(MouseEventArgs args)
        {
            //JBH await SignOutManager.SetSignOutState();
            Navigation.NavigateTo("Account/logout");
        }
    }
}