using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Timers;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading;
using System.Text.Json.Serialization;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;

using Telerik.Blazor;
using Telerik.Blazor.Components;
using Telerik.DataSource;
using Telerik.Generated.Blazor.Components;

using AutoMapper;

using Fims.Client.Shared.ClientModels;
using Fims.Client.Shared.ClientServices.TSheetSpecsInProgress;
using Fims.Client.Shared.Infrastructure.Extensions;
using Fims.Common;
using Fims.Data.Models.TSheetSpecs;
using Fims.Data.Utils;
using Fims.Data.Models.TSheetSpecsInProgress;
using Microsoft.AspNetCore.Components.Authorization;
using Fims.Client.Shared.Pages.Account;
using Azure;

namespace Fims.Client.Shared.Pages
{
    public partial class InspectCloser
    {
        //[CascadingParameter]
        //public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        private List<string>                   ProductSerials { get; set; } = new List<string>();
        private Dictionary<string, TSheetSpec> ProductSerialToTSheetSpecDict { get; set; } = new Dictionary<string, TSheetSpec>();
        private Dictionary<string, bool>       ProductSerialsSelected { get; set; } = new Dictionary<string, bool>();

        //private List<string> ProductModels { get; set; } = new List<string>();

        private TSheetSpec CurrentTSheetSpec { get; set; }
        private string     CurrentCloserName { get; set; }
        private string     CurrentCloserUserId { get; set; }

        public bool AddNewProductDialogVisible { get; set; } = false;

        public int Page { get; set; } = 1;

        public AddNewProductModal TheAddNewProductModal { get; set; }
        TelerikNotification LoadSessionNotificationComponent { get; set; }
        public List<string> ToggleButtonsThemeColor { get; set; }

#nullable enable  //suppress the Warning CS8632
        //private System.Timers.Timer TSheetSpecsSavingTimer;
        private System.Threading.Timer? TSheetSpecsSavingTimer2;
#nullable disable

        public bool IsLoadingSession { get; set; } = false;
        public bool IsSavingSession { get; set; } = false;


        protected override void OnInitialized()
        {
            Layout.DocsTitle = Localizer["HumanCapital"];

            //FIXME: Causing an Exception Now:
            // TSheetSpecsSavingTimer = new();
            // TSheetSpecsSavingTimer.Interval = 1000 * 20; // every 20 secs
            // TSheetSpecsSavingTimer.Elapsed += async (object sender, ElapsedEventArgs e) =>
            // {
            //     OnSaveSessionDataByTimer();
            //     await InvokeAsync(StateHasChanged);
            //     await Task.Delay(1); // for async
            // };
            // TSheetSpecsSavingTimer.Enabled = true;

            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            //FIXME    // Accessing LocalStorage at this phase is not allowed. JSRuntime out of WebView.
            //FIXME    // So do it after rendering finished.
            //FIXME    var state = await this.AuthStateProvider.GetAuthenticationStateAsync();
            //FIXME    var user = state.User;
            //FIXME    //var authState = await AuthenticationStateTask;
            //FIXME    //var user = authState.User;
            //FIXME    CurrentCloserName = user.GetHangulName();
            //FIXME    
            //FIXME    ProductModels = await TSheetSpecsClientService.GetEquipmentModelsAsync();

#if !DEBUG
            TSheetSpecsSavingTimer2 = new System.Threading.Timer(async (object? stateInfo) =>
            {
                OnSaveSessionDataByTimer();
                // NOTE: must call StateHasChanged() because this is triggered by a timer instead of a user event.
                await InvokeAsync(StateHasChanged);  //NOTE: Direct calling StateHasChanged() without InvokeAsync causes an Exception.
            }, new System.Threading.AutoResetEvent(false), 1000 * 60, 1000 * 60); // fire every 60 secs
#endif

            await base.OnInitializedAsync();
        }

        protected override async Task OnParametersSetAsync()
        {
            Console.WriteLine("Index: OnParametersSetAsync called");
            await base.OnParametersSetAsync();
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
                var state = await this.AuthStateProvider.GetAuthenticationStateAsync();
                var user = state.User;
                //var authState = await AuthenticationStateTask;
                //var user = authState.User;

                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //JBH FIXME: null upon right after logged in. why?
                CurrentCloserName = user.GetHangulName();
                CurrentCloserUserId = user.GetUserId();
                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                //ProductModels = await TSheetSpecsClientService.GetEquipmentModelsAsync();
            }
        }

        public void OnProductSerialButtonGroupClick(string productSerial)
        {
            SetProductSerialAsCurrent(productSerial);
            //StateHasChanged();
        }

        public void OnTSheetClosingCompleted(string productSerial)
        {
            var tSheetSpec = ProductSerialToTSheetSpecDict[productSerial];
            ProductSerialToTSheetSpecDict[productSerial].IsClosingCompleted = true;

            TSheetSpecsInCloseClientService.DeleteTSheetSpecsInCloseByUserIdProductSerial(productSerial);

            ProductSerials.Remove(productSerial);
            ProductSerialsSelected.Remove(productSerial);
            ProductSerialToTSheetSpecDict.Remove(productSerial);

            var firstEntry = ProductSerialToTSheetSpecDict.FirstOrDefault();
            if (firstEntry.Key != null)
            {
                SetProductSerialAsCurrent(firstEntry.Key);
            }
            else
            {
                // Products empty, so no display of TSheetComponent
                CurrentTSheetSpec = null;
            }

            StateHasChanged();
        }

        private void SetProductSerialAsCurrent(string productSerial)
        {
            var tSheetSpecSelected = ProductSerialToTSheetSpecDict[productSerial] as TSheetSpec;

            tSheetSpecSelected.IsInInspecting = false;
            tSheetSpecSelected.IsInClosing = true;
            if (tSheetSpecSelected.CloserName.IsNullOrEmpty())
            {
                tSheetSpecSelected.CloserName = CurrentCloserName;
                tSheetSpecSelected.ClosingStartDateTime = DateTime.Now;
            }

            ProductSerialsSelected.Keys.ToList().ForEach(serial =>{ProductSerialsSelected[serial] = false;});
            ProductSerialsSelected[productSerial] = true;

            CurrentTSheetSpec = tSheetSpecSelected;

            StateHasChanged();
        }

        public async void OnLoadSessionData()
        {
            IsLoadingSession = true;

            var state = await this.AuthStateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            //var authState = await AuthenticationStateTask;
            //var user = authState.User;
            CurrentCloserName = user.GetHangulName();
            CurrentCloserUserId = user.GetUserId();

            TSheetSpecsInProgressDto tSheetSpecsInCloseDto = await TSheetSpecsInCloseClientService.GetTSheetSpecsInCloseByUser(CurrentCloserUserId);
            if (tSheetSpecsInCloseDto.UserId.StartsWith("HTTPFAIL"))
            {
                LoadSessionNotificationComponent.Show(new NotificationModel()
                {
                    Text = "가저오기 실패: FIMS서버 연결에 문제가 있습니다.",
                    CloseAfter = 3000,
                    ThemeColor = "warning",
                    ShowIcon = true,
                    Icon = "caret-double-alt-down"
                });
                IsLoadingSession = false;
                StateHasChanged();
                return;
            }

            var userIdRx = tSheetSpecsInCloseDto.UserId;
            var serialToTSheetSpecPairs = tSheetSpecsInCloseDto.SerialToTSheetSpecPairs;

            if (serialToTSheetSpecPairs.Count == 0)
            {
                LoadSessionNotificationComponent.Show(new NotificationModel()
                {
                    Text = "저장된 진행목록이 없습니다.",
                    CloseAfter = 3000,
                    ThemeColor = "warning",
                    ShowIcon = true,
                    Icon = "caret-double-alt-down"
                });
                IsLoadingSession = false;
                StateHasChanged();
                return;
            }

            foreach (var serialToTSheetSpecPair in serialToTSheetSpecPairs)
            {
                var productSerial = serialToTSheetSpecPair.Key;
                var tSheetSpecJsonString = serialToTSheetSpecPair.Value;

                MemoryStream tSheetSpecJsonStream = new MemoryStream(Encoding.UTF8.GetBytes(tSheetSpecJsonString));
                var tSheetSpec = await JsonSerializer.DeserializeAsync<TSheetSpec>(tSheetSpecJsonStream);

                if (!ProductSerialToTSheetSpecDict.ContainsKey(productSerial))
                {
                    //tSheetSpec.IsInInspecting = false;
                    //tSheetSpec.IsInClosing = true;

                    ProductSerialToTSheetSpecDict?.Add(productSerial, tSheetSpec);
                    ProductSerialsSelected?.Add(productSerial, false);
                    ProductSerials?.Add(productSerial);
                }
            }

            //SetProductSerialAsCurrent(tProductSpec.ProductSerial);

            // LoadSessionNotificationComponent.Show(new NotificationModel()
            // {
            //     Text = "진행목록이 성공적으로 로딩되었습니다.",
            //     CloseAfter = 2000,
            //     ThemeColor = "primary",
            //     ShowIcon = true,
            //     Icon = "caret-double-alt-down"
            // });

            IsLoadingSession = false;
            StateHasChanged();
        }

        public async void OnSaveSessionData()
        {
            IsSavingSession = true;
 
            if (ProductSerialToTSheetSpecDict.Count == 0)
            {
                LoadSessionNotificationComponent.Show(new NotificationModel()
                {
                    Text = "진행목록이 비어 있습니다.",
                    CloseAfter = 3000,
                    ThemeColor = "warning",
                    ShowIcon = true,
                    Icon = "caret-double-alt-up"
                });
                IsSavingSession = false;
                StateHasChanged();
                return;
            }

            bool result = await SaveSessionData();
            if (result)
            {
                IsSavingSession = false;
                LoadSessionNotificationComponent.Show(new NotificationModel()
                {
                    Text = "진행목록이 성공적으로 저장되었습니다.",
                    CloseAfter = 2000,
                    ThemeColor = "success",
                    ShowIcon = true,
                    Icon = "caret-double-alt-up"
                });
            }
            else
            {
                IsSavingSession = false;
                LoadSessionNotificationComponent.Show(new NotificationModel()
                {
                    Text = "저장실패: FIMS서버 연결에 문제가 있습니다.",
                    CloseAfter = 3000,
                    ThemeColor = "warning",
                    ShowIcon = true,
                    Icon = "caret-double-alt-up"
                });
            }

            IsSavingSession = false;
            StateHasChanged();
        }

        public async void OnSaveSessionDataByTimer()
        {
            //JBH NOTE: This is running on the NON-UI thread.
            //          Do not do any UI things (such as StateHasChanged) here.
            //          if needed, use InvokeAsync().
            //          https://blazor-university.com/components/multi-threaded-rendering/invokeasync/

            if (ProductSerialToTSheetSpecDict.Count == 0)
            {
                // StateHasChanged();
                return;
            }

            bool result = await SaveSessionData();
            // if ( !result )
            // {
            //     LoadSessionNotificationComponent.Show(new NotificationModel()
            //     {
            //         Text = "저장실패: FIMS서버 연결에 문제가 있습니다.",
            //         CloseAfter = 3000,
            //         ThemeColor = "warning",
            //         ShowIcon = true,
            //         Icon = "caret-double-alt-up"
            //     });
            // }

            //StateHasChanged();
        }

        public async Task<bool> SaveSessionData()
        {
            IsSavingSession = true;

            if (ProductSerialToTSheetSpecDict.Count == 0)
            {
                IsSavingSession = false;

                //JBH: 
                //     When SaveSessionData() called from OnSaveSessionDataByTimer() which is invoked by the timer thread,
                //     SaveSessionData() is running on the NON-UI thread!
                //     Make sure call by InvokeAsync.
                //     If not, the error: "The current thread is not associated with the Dispatcher. Use InvokeAsync()"
                //     https://blazor-university.com/components/multi-threaded-rendering/invokeasync/
                await InvokeAsync(StateHasChanged);
                return false;
            }

            var state = await this.AuthStateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            //var authState = await AuthenticationStateTask;
            //var user = authState.User;

            CurrentCloserName = user.GetHangulName();
            CurrentCloserUserId = user.GetUserId();

            TSheetSpecsInProgressDto tSheetSpecsInCloseReqeust = new TSheetSpecsInProgressDto
            {
                UserId = CurrentCloserUserId,
                SerialToTSheetSpecPairs = new Dictionary<string, string>()
            };

            int countInProgress = 0;

            foreach (var productSerialToTSheetSpec in ProductSerialToTSheetSpecDict)
            {
                var serial = productSerialToTSheetSpec.Key;
                var tSheetSpec = productSerialToTSheetSpec.Value;
                if ( !tSheetSpec.IsClosingCompleted )
                {
                    // save "In-Closing" inspections only. do not save "Completed" inspections
                    countInProgress++;
                    var jsonString = JsonUtils.PrettySerialize(tSheetSpec);
                    tSheetSpecsInCloseReqeust.SerialToTSheetSpecPairs.Add(serial, jsonString);
                }
            }

            if (countInProgress == 0)
            {
                IsSavingSession = false;
                await InvokeAsync(StateHasChanged);
                return false;
            }

            var fileName = await TSheetSpecsInCloseClientService.SaveTSheetSpecsInCloseByUser(tSheetSpecsInCloseReqeust);
            if ( fileName == null )
            {
                LoadSessionNotificationComponent.Show(new NotificationModel()
                {
                    Text = "저장실패: FIMS서버 연결에 문제가 있습니다.",
                    CloseAfter = 3000,
                    ThemeColor = "warning",
                    ShowIcon = true,
                    Icon = "caret-double-alt-up"
                });

                IsSavingSession = false;
                await InvokeAsync(StateHasChanged);
                return false;
            }
            else
            {
                IsSavingSession = false;
                await InvokeAsync(StateHasChanged);
                return true;
            }
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
