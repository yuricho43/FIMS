using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using Telerik.Blazor;
using Telerik.Blazor.Components;
using Telerik.DataSource;
using static Telerik.Blazor.ThemeConstants;

using AutoMapper;

using Fims.Client.Shared.ClientModels;
using Fims.Client.Shared.ClientServices.TSheetSpecs;
using Fims.Common;
using Fims.Common.Mapping;
using Fims.Data.Entities;
using Fims.Data.Models;
using Fims.Data.Models.Identity;
using Fims.Data.Models.TSheetSpecs;
using Fims.Client.Shared.ClientServices.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Fims.Data.Models.TReports;
using Azure;
using Microsoft.JSInterop;
using Telerik.SvgIcons;

namespace Fims.Client.Shared.Pages.Management
{
    public partial class GenerateTReportDialog
    {
        //[CascadingParameter]
        //public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        [Parameter]
        public List<string> TReportSpecs { get; set; }

        [Parameter]
        public int TSheetId { get; set; }

        [Parameter]
        public EventCallback<string> GenerateTReportFinished { get; set; }

        public TReportDto NewTReportRequestDto { get; set; } = new TReportDto();

        public TelerikForm GenerateTReportFormRef { get; set; }
        public TelerikNotification GenerateTReportNotificationComponent { get; set; }
        public bool ShowErrors { get; set; }
        public IEnumerable<string> Errors { get; set; }


        protected override void OnInitialized()
        {
            NewTReportRequestDto.TSheetId = TSheetId;
            base.OnInitialized();
        }

        //protected override async Task OnInitializedAsync()
        //{
        //    var state = await this.AuthStateProvider.GetAuthenticationStateAsync();
        //    var user = state.User;
        //    //var authState = await AuthenticationStateTask;
        //    //var user = authState.User;
        //
        //    ProductModels = await TSheetSpecsClientService.GetEquipmentModelsAsync();
        //}

        public FormValidationMessageType ValidationMessageType { get; set; } = FormValidationMessageType.Tooltip;
        public List<FormValidationMessageType> ValidationMessageTypes { get; set; } = new List<FormValidationMessageType>()
        {
            FormValidationMessageType.None,
            FormValidationMessageType.Inline,
            FormValidationMessageType.Tooltip
        };
        public bool ValidSubmit { get; set; } = false;

        async void HandleValidSubmit()
        {
            ValidSubmit = true;

            var tReportSpecBase = NewTReportRequestDto.TReportSpec.Split('_').ToList()[0]; // remove date from "CHILLER 검사 성적서_20221226"
            var tReportOutputFile = $"FimsReport_T{TSheetId}_{tReportSpecBase}_{DateTime.Now:yyyyMMddhhmmss}.xlsx";
            TReportDto newReportRequest = new TReportDto
            {
                TSheetId = TSheetId,
                TReportSpec =       $"FimsTReportSpecs_{NewTReportRequestDto.TReportSpec}.xlsx",
                TReportOutputFile = tReportOutputFile,
                IsSuccess = false,
            };

            var newTReportResponse = await TReportsClientService.GenerateTReport(newReportRequest);

            if (!newTReportResponse.IsSuccessStatusCode)
            {
                await JS.InvokeVoidAsync("alert", "File not found.");
            }
            else
            {
                var fileStream = newTReportResponse.Content.ReadAsStream();
                using var streamRef = new DotNetStreamReference(stream: fileStream);
                await JS.InvokeVoidAsync("downloadFileFromStream", tReportOutputFile, streamRef);
            }


            // if (tReportGenerated.IsSuccess)
            // {
            //     this.ShowErrors = false;
            // 
            //     GenerateTReportNotificationComponent.Show(new NotificationModel
            //     {
            //         Text = "성적서 생성 성공",
            //         ThemeColor = "error",
            //         CloseAfter = 3000
            //     });
            // 
            //     await GenerateTReportFinished.InvokeAsync(NewTReportRequestDto.TReportOutputFile); // pass Param to parent, by calling EventCallback
            //     //this.NavigationManager.NavigateTo("/account/login");
            // }
            // else
            // {
            //     //this.Errors = result.Errors;
            //     this.ShowErrors = true;
            // }

            await GenerateTReportFinished.InvokeAsync(NewTReportRequestDto.TReportOutputFile); // pass Param to parent, by calling EventCallback
            this.ShowErrors = false;
            ValidSubmit = false;

            StateHasChanged();

            /*
             * A Downloaded File goes inside of 
             *      - MauiApp(WebView): the %USERPROFILE%\Downloads directory
             *      - WebApp:           the %USERPROFILE%\Downloads directory or the download directory set by user.
             */
        }

        void HandleInvalidSubmit()
        {
            ValidSubmit = false;
        }

        void OnAddUserDialogOK()
        {
            //ValidSubmit = false;
        }

        void OnGenerateTReportDialogCancel()
        {
            //ValidSubmit = false;
            GenerateTReportFinished.InvokeAsync(NewTReportRequestDto.TReportOutputFile); // pass Param to parent, by calling EventCallback
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
