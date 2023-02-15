using System.Security.Cryptography;
using System.Text;
using Fims.Client.Shared.Infrastructure.Extensions;
using Fims.Client.Shared.Pages.Account;
using Fims.Data.Models.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;


namespace Fims.Client.Shared.Shared.NavMenu
{
    public partial class LoginDisplay
    {
        //[CascadingParameter]
        //public Task<AuthenticationState> AuthenticationStateTask { get; set; }
        public bool UserProfileDialogVisible { get; set; } = false;

        public string UserName { get; set; }

        protected override async Task OnInitializedAsync()
        {
            //FIXME    // Accessing LocalStorage at this phase is not allowed. JSRuntime out of WebView.
            //FIXME    // So do it after rendering finished.
            //FIXME    var state = await this.AuthState.GetAuthenticationStateAsync();
            //FIXME    var user = state.User;
            //FIXME    //var authState = await AuthenticationStateTask;
            //FIXME    //var user = authState.User;
            //FIXME    if (user.Identity.IsAuthenticated)
            //FIXME    {
            //FIXME        UserName = user.GetUserName();
            //FIXME    }

           await base.OnInitializedAsync();
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
                var user = state.User;
                //var authState = await AuthenticationStateTask;
                //var user = authState.User;

                if (user.Identity.IsAuthenticated)
                {
                    UserName = user.GetUserName();
                }
                StateHasChanged();
            }
        }

        private RenderFragment DynamicRender { get; set; }

        private RenderFragment CreateComponent() => builder =>
        {
            builder.OpenComponent(0, typeof(UserProfileDialog));
            builder.AddAttribute(1, "UserProfileDialogFinished", "OnUserProfileDialogFinished");
            builder.CloseComponent();
        };


        private void ShowUserProfileDialogVisible()
        {
            CreateComponent();

            //ProductService.UpdateProduct((ProductDto)args.Item);
            //await LoadData();
            UserProfileDialogVisible = true;
            //StateHasChanged();
        }

        private void OnUserProfileDialogFinished()
        {
            //builder.CloseComponent();
            UserProfileDialogVisible = false;
            //StateHasChanged();
        }

        private void BeginLogIn(MouseEventArgs args)
        {
            //JBH await SignOutManager.SetSignOutState();
            Navigation.NavigateTo("Account/login");
        }

        private void BeginLogOut(MouseEventArgs args)
        {
            //JBH await SignOutManager.SetSignOutState();
            Navigation.NavigateTo("Account/logout");
        }
    }
}