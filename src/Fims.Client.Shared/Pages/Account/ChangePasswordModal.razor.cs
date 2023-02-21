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
    public partial class ChangePasswordModal
    {
        //[CascadingParameter]
        //public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        [Parameter]
        public EventCallback<bool> ChangePasswordDialogFinished { get; set; }

        private readonly PasswordModel ChangePasswordModel = new PasswordModel();
        //private string Email;

        public bool ShowErrors { get; set; } = false;

        public IEnumerable<string> Errors { get; set; }

        TelerikNotification ChangePasswordNotificationComponent { get; set; }


        public bool ValidSubmit { get; set; } = false;

        async void HandleValidSubmit()
        {
            ValidSubmit = true;

            var result = await this.AuthClientService.ChangePassword(this.ChangePasswordModel);

            if (result.Succeeded)
            {
                this.ShowErrors = false;
                //
                //this.ChangePasswordModel.Password = null;
                //this.ChangePasswordModel.NewPassword = null;
                //this.ChangePasswordModel.ConfirmNewPassword = null;
                //
                //await this.AuthClientService.Logout();
                //
                ////this.ToastService.ShowSuccess("Your password has been changed successfully.\n Please login.");
                //this.NavigationManager.NavigateTo("/account/login");

                //ChangePasswordNotificationComponent.Show(new NotificationModel()
                //{
                //    Text = "암호가 성공적으로 변경되었습니다.",
                //    ThemeColor = "primary",
                //    ShowIcon = true,
                //    Icon = "caret-double-alt-up"
                //});

                await ActivateAlert("암호변경", "성공적으로 변경되었습니다.");

                //StateHasChanged();

                //await ChangePasswordDialogFinished.InvokeAsync(true); // pass Param to parent, by calling EventCallback
            }
            else
            {
                this.Errors = result.Errors;
                this.ShowErrors = true;
                await ActivateAlert("암호변경 실패", this.Errors.FirstOrDefault());
            }

            ValidSubmit = false;

            //StateHasChanged();
        }

        void HandleInvalidSubmit()
        {
            ValidSubmit = false;
        }

        private async void OnCancel()
        {
            await ChangePasswordDialogFinished.InvokeAsync(false); // pass Param to parent, by calling EventCallback
            //StateHasChanged();
        }


        [CascadingParameter]
        public DialogFactory Dialogs { get; set; }
        public async Task ActivateAlert(string title, string message)
        {
            if (string.IsNullOrWhiteSpace(title)) title = "Warning!";
            if (string.IsNullOrWhiteSpace(message)) message = "Something went wrong!";

            await Dialogs.AlertAsync(message, title);
        }

    }
}