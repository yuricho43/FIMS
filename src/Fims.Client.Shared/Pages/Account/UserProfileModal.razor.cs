using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Microsoft.Extensions.Localization;
using Telerik.DataSource;
using Telerik.Blazor;
using Telerik.Blazor.Components;
using Telerik.Blazor.Components.Common.Icon;
using Telerik.Blazor.Services;
using Telerik.FontIcons;
using Fims.Common;
using Fims.Data.Models;
using Fims.Data.Models.TSheetSpecs;
using Fims.Client.Shared;
using Fims.Client.Shared.Infrastructure;
using Fims.Client.Shared.Infrastructure.Extensions;
using Fims.Client.Shared.ClientServices;
using Fims.Client.Shared.ClientServices.Authentication;
using Fims.Client.Shared.ClientServices.TSheets;
using Fims.Client.Shared.ClientServices.TSheetSpecs;
using Fims.Client.Shared.ClientServices.TSheetSpecsInProgress;
using Fims.Client.Shared.ClientServices.TReports;
using Fims.Client.Shared.Pages;
using Fims.Client.Shared.Pages.Account;
using Fims.Client.Shared.Shared;
using Fims.Client.Shared.Shared.Common;
using Fims.Client.Shared.Shared.Layouts;
using Fims.Data.Models.Identity;

namespace Fims.Client.Shared.Pages.Account
{
    public partial class UserProfileModal
    {
        //[CascadingParameter]
        //public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        [Parameter]
        public EventCallback<bool> UserProfileDialogFinished { get; set; }

        [Parameter]
        public bool DialogVisible { get; set; } = false;

        //private UserProfileModel UserProfileModel = new UserProfileModel();
        public UserProfileModel CurrentUserProfileModel { get; set; } = new UserProfileModel();

        public bool ChangePasswordDialogVisible { get; set; } = false;

        public bool ShowErrors { get; set; }
        public IEnumerable<string> Errors { get; set; }


        protected override async Task OnInitializedAsync()
        {
            //await this.LoadDataAsync();
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
                await this.LoadDataAsync();
                //StateHasChanged();
            }
        }

        private async Task SubmitAsync()
        {
            var result = await this.AuthClientService.ChangeProfile(this.CurrentUserProfileModel);

            if (result.Succeeded)
            {
                this.ShowErrors = false;
                await this.AuthClientService.Logout();
                //this.ToastService.ShowSuccess("Your account UserProfile has been changed successfully.\n Please login.");
                this.NavigationManager.NavigateTo("/account/login");
            }
            else
            {
                this.Errors = result.Errors;
                this.ShowErrors = true;
            }
        }
        public bool ValidSubmit { get; set; } = false;

        async void HandleValidSubmit()
        {
            ValidSubmit = true;

            await Task.Delay(2000);

            ValidSubmit = false;

            StateHasChanged();
        }

        void HandleInvalidSubmit()
        {
            ValidSubmit = false;
        }

        private async void OnConfirm()
        {
            await UserProfileDialogFinished.InvokeAsync(true); // pass Param to parent, by calling EventCallback
            //StateHasChanged();
        }

        private void OnChangePasswordClicked()
        {
            ChangePasswordDialogVisible = true;
        }

        private void OnChangePasswordDialogFinished(bool result)
        {
            ChangePasswordDialogVisible = false;
        }

        private async Task LoadDataAsync()
        {
            var state = await this.AuthState.GetAuthenticationStateAsync();
            var user = state.User;
            //var authState = await AuthenticationStateTask;
            //var user = authState.User;
            this.CurrentUserProfileModel.UserName = user.GetUserName();
            this.CurrentUserProfileModel.Email = user.GetEmail();
            this.CurrentUserProfileModel.HangulName = user.GetHangulName();
            //this.UserProfileModel.EnglishName = user.GetEnglishName();
            this.CurrentUserProfileModel.Role = user.GetUserRole();

            //this.NavigationManager.NavigateTo("/account/userprofiledialog", forceLoad: false);
            StateHasChanged();
        }
    }
}