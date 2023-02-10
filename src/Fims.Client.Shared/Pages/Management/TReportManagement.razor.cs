using System.Net.Http.Json;
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
using Telerik.Blazor.Components.FileSelect;

using AutoMapper;

using Fims.Common;
using Fims.Data.Entities;
using Fims.Data.Models;

namespace Fims.Client.Shared.Pages.Management
{
    public partial class TReportManagement
    {
        public List<string> AllowedExtensions { get; set; } = new List<string>() { ".xlsx" };
        public List<FileSelectFileInfo> FileSelectFileInfos { get; set; } = new List<FileSelectFileInfo>();

        public TelerikFileSelect TReportFileSelector { get; set; }
        TelerikNotification UploadTReportNotificationComponent { get; set; }


        protected override void OnInitialized()
        {
        }

        private void OnFileSelected(FileSelectEventArgs args)
        {
            foreach (var file in args.Files)
            {
                if (!file.InvalidExtension && file.Name.StartsWith("FimsTReportSpecs_"))
                {
                    FileSelectFileInfos.Add(file);
                }
            }
        }

        public async Task OnUploadSpec()
        {
            await UploadSpecFiles();
        }

        private List<string> FileNamesToUpload = new();

        private async Task UploadSpecFiles()
        {
            if (FileSelectFileInfos.Count < 1) return;

            foreach (var file in FileSelectFileInfos)
            {
                if (!file.InvalidExtension)
                {
                    using var content = new MultipartFormDataContent();
                    var fileContent = new StreamContent(file.Stream);
                    //fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                    FileNamesToUpload.Add(file.Name);
                    content.Add(content: fileContent, name: "\"files\"", fileName: file.Name);
                    var response = await Http.PostAsync("api/TReports/save", content);
                    var uploadResult = await response.Content.ReadAsStringAsync();

                    if (uploadResult.StartsWith("SUCCESS"))
                    {
                        UploadTReportNotificationComponent.Show(new NotificationModel()
                        {
                            Text = "성적서스펙 파일이 성공적으로 교체되었습니다.",
                            ThemeColor = "primary",
                            ShowIcon = true,
                            Icon = "caret-double-alt-up"
                        });

                        StateHasChanged();
                    }
                    else
                    {
                        _ = ActivateAlert("교체 실패", uploadResult);
                    }

                }
            }

            //await RemoveSecondFile();

            FileSelectFileInfos.Clear(); //remove from the selected files list
            int cool = 7;
        }

        public Dictionary<string, CancellationTokenSource> Tokens { get; set; } = new Dictionary<string, CancellationTokenSource>();
        private async Task ReadFile(FileSelectFileInfo file)
        {
            Tokens.Add(file.Id, new CancellationTokenSource());
            var byteArray = new byte[file.Size];
            await using MemoryStream ms = new MemoryStream(byteArray);
            await file.Stream.CopyToAsync(ms, Tokens[file.Id].Token);
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
