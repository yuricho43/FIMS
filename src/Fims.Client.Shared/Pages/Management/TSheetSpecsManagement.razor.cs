using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;

using Telerik.Blazor;
using Telerik.Blazor.Components;
using Telerik.Blazor.Components.Upload;
using Telerik.DataSource;

using AutoMapper;

using Fims.Client.Shared.ClientServices.TSheetSpecs;
using Fims.Common;
using Fims.Data.Entities;
using Fims.Data.Models;
using Fims.Data.Models.TSheetSpecs;

namespace Fims.Client.Shared.Pages.Management
{
    public partial class TSheetSpecsManagement
    {
        // setup upload endpoints
        public string SaveUrl => ToAbsoluteUrl("api/TSheetSpecs/save");
        public string RemoveUrl => ToAbsoluteUrl("api/TSheetSpecs/remove");

        public string ToAbsoluteUrl(string url)
        {
            return $"{NavigationManager.BaseUri}{url}";
        }

        // setup validation, this is a workaround because file validation does not exist in the framework
        // see more here https://github.com/dotnet/aspnetcore/issues/18821
        JobApplicationForm currentForm { get; set; }
        protected EditContext MyEditContext { get; set; }

        private bool NotValid => MyEditContext.GetValidationMessages().Any();
        Dictionary<string, bool> FilesValidationInfo { get; set; } = new Dictionary<string, bool>();

        protected override void OnInitialized()
        {
            currentForm = new JobApplicationForm();
            MyEditContext = new EditContext(currentForm);
        }

        void OnSelectHandler(UploadSelectEventArgs e)
        {
            foreach (var item in e.Files)
            {
                if (!FilesValidationInfo.Keys.Contains(item.Id))
                {
                    // nothing is assumed to be valid until the server returns an OK
                    FilesValidationInfo.Add(item.Id, IsSelectedFileValid(item));
                }
            }

            UpdateValidationModel();
        }

        void OnSuccessHandler(UploadSuccessEventArgs e)
        {
            if (e.Operation == UploadOperationType.Upload)
            {
                if (FilesValidationInfo.Keys.Contains(e.Files[0].Id))
                {
                    // only when the server got the file, saved it and confirmed it is OK do we update client validation
                    FilesValidationInfo[e.Files[0].Id] = true;
                }
            }
            else
            {
                RemoveFailedFilesFromList(e.Files);
            }

            UpdateValidationModel();
        }

        void OnRemoveHandler(UploadEventArgs e)
        {
            RemoveFailedFilesFromList(e.Files);
            UpdateValidationModel();
        }

        void OnErrorHandler(UploadErrorEventArgs e)
        {
            RemoveFailedFilesFromList(e.Files);
            UpdateValidationModel();
        }

        void OnCancelHandler(UploadCancelEventArgs e)
        {
            RemoveFailedFilesFromList(e.Files);
            UpdateValidationModel();
        }

        void RemoveFailedFilesFromList(List<UploadFileInfo> files)
        {
            foreach (var file in files)
            {
                if (FilesValidationInfo.Keys.Contains(file.Id))
                {
                    FilesValidationInfo.Remove(file.Id);
                }
            }
        }

        bool IsSelectedFileValid(UploadFileInfo file)
        {
            return !(file.InvalidExtension || file.InvalidMaxFileSize || file.InvalidMinFileSize);
        }

        void UpdateValidationModel()
        {
            bool areAllUploadedFilesValid = false;

            if (FilesValidationInfo.Keys.Count > 0 &&
                !FilesValidationInfo.Values.Contains(false))
            {
                areAllUploadedFilesValid = true;
            }

            currentForm.IsSpecFileValid = areAllUploadedFilesValid;

            // we update the validation state out of the standard form cycle and events
            // so we need an EditContext that we can call upon to re-evaluate the validation
            MyEditContext.Validate();
        }


        // sample model
        public class JobApplicationForm
        {
            //[Required(ErrorMessage = "Enter your name")]
            //public string Name { get; set; }

            //[Required(ErrorMessage = "Enter your email")]
            //[EmailAddress(ErrorMessage = "Please provide a valid email address.")]
            //public string Email { get; set; }

            [Required(ErrorMessage = "FimsTSheetSpecs_YYYYMMDD.xlsx 형식의 파일을 선택하세요")]
            [Range(typeof(bool), "true", "true", ErrorMessage = "FimsTSheetSpecs_YYYYMMDD.xlsx 형식의 파일을 선택하세요")]
            public bool IsSpecFileValid { get; set; }
        }

        // UI for the demo to showcase changes to the form validation and success
        string SuccessMessage = string.Empty;

        void HandleValidSubmit()
        {
            SuccessMessage = "Spec Sheets 파일을 성공적으로 교체하였습니다.";
        }

        void HandleInvalidSubmit()
        {
            SuccessMessage = string.Empty;
        }

        async Task BackToForm()
        {
            await JsInterop.InvokeVoidAsync("window.location.reload");
        }
    }
}
