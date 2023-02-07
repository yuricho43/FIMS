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
using Telerik.Blazor.Components.FileSelect;

namespace Fims.Client.Shared.Pages.Management
{
    public partial class FileSelectEvents
    {
        public string EventLog { get; set; } = "";
        void OnClearLogClick()
        {
            EventLog = string.Empty;
        }

        public void OnSelect(FileSelectEventArgs args)
        {
            var log = "<div><strong>Select event</strong>. The following files have been selected:<ul>";
            foreach (var file in args.Files)
            {
                log += $"<li><strong>{file.Name}</strong></li>";
            }

            log += "</ul></div>";
            EventLog = EventLog.Insert(0, log);
        }

        public void OnRemove(FileSelectEventArgs args)
        {
            var log = "<div><strong>Remove event</strong>. The following files will be removed:<ul>";
            foreach (var file in args.Files)
            {
                log += $"<li><strong>{file.Name}</strong></li>";
            }

            log += "</ul></div>";
            EventLog = EventLog.Insert(0, log);
        }

        public async Task OnUploadSpec()
        {
            await UploadSpecFiles();
        }

        public List<string> AllowedExtensions { get; set; } = new List<string>()
        {
            ".jpg",
            ".png",
            ".gif"
        };
        public Dictionary<string, CancellationTokenSource> Tokens { get; set; } = new Dictionary<string, CancellationTokenSource>();

        public List<FileSelectFileInfo> FileSelectFileInfos { get; set; } = new List<FileSelectFileInfo>();

        private void HandleFiles(FileSelectEventArgs args)
        {
            FileSelectFileInfos = args.Files;
        }

        private async Task UploadSpecFiles()
        {
            //if (FileSelectFileInfos.Count < 1) return;

            foreach (var file in FileSelectFileInfos)
            {
                if (!file.InvalidExtension)
                {
                    using var content = new MultipartFormDataContent();
                    var fileContent = new StreamContent(file.Stream);
                    //fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                    fileNames.Add(file.Name);
                    content.Add(content: fileContent, name: "\"files\"", fileName: file.Name);
                    var response = await Http.PostAsync("api/TSheetSpecs/save", content);
                    var newUploadResults = await response.Content.ReadFromJsonAsync<List<UploadResult>>();
                    if (newUploadResults is not null)
                    {
                        uploadResults = uploadResults.Concat(newUploadResults).ToList();
                    }
                }
            }
        }

        // private async Task UploadFile(FileSelectFileInfo file)
        // {
        //     Tokens.Add(file.Id, new CancellationTokenSource());
        //     var path = Path.Combine(HostingEnvironment?.WebRootPath, file.Name);
        //     await using FileStream fs = new FileStream(path, FileMode.Create);
        //     await file.Stream.CopyToAsync(fs, Tokens[file.Id].Token);
        // 
        // 
        // }
        private async Task ReadFile(FileSelectFileInfo file)
        {
            Tokens.Add(file.Id, new CancellationTokenSource());
            var byteArray = new byte[file.Size];
            await using MemoryStream ms = new MemoryStream(byteArray);
            await file.Stream.CopyToAsync(ms, Tokens[file.Id].Token);
        }

        // private async Task HandleRemoveFiles(FileSelectEventArgs args)
        // {
        //     foreach (var file in args.Files)
        //     {
        //         // If you're still uploading the file, cancel the process first.
        //         Tokens[file.Id].Cancel();
        //         Tokens.Remove(file.Id);
        // 
        //         await Task.Delay(1);
        // 
        //         var path = Path.Combine(HostingEnvironment?.WebRootPath, file.Name);
        // 
        //         // Remove the file from the file system
        //         File.Delete(path);
        //     }
        // 
        // }
        private int maxAllowedFiles = int.MaxValue;
        private long maxFileSize = long.MaxValue;
        private List<string> fileNames = new();
        private List<UploadResult> uploadResults = new();
        private async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            using var content = new MultipartFormDataContent();
            foreach (var file in e.GetMultipleFiles(maxAllowedFiles))
            {
                var fileContent = new StreamContent(file.OpenReadStream(maxFileSize));
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                fileNames.Add(file.Name);
                content.Add(content: fileContent, name: "\"files\"", fileName: file.Name);
            }

            var response = await Http.PostAsync("/api/File", content);
            var newUploadResults = await response.Content.ReadFromJsonAsync<List<UploadResult>>();
            if (newUploadResults is not null)
            {
                uploadResults = uploadResults.Concat(newUploadResults).ToList();
            }
        }

        public class UploadResult
        {
            public int Id { get; set; }

            public string FileName { get; set; }

            public string StoredFileName { get; set; }

            public string ContentType { get; set; }
        }
    }
}