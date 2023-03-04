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
        private string     CurrentInspectorName { get; set; }
        private string     CurrentInspectorUserId { get; set; }

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
            //FIXME    CurrentInspectorName = user.GetHangulName();
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
                CurrentInspectorName = user.GetHangulName();
                CurrentInspectorUserId = user.GetUserId();
                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                //ProductModels = await TSheetSpecsClientService.GetEquipmentModelsAsync();
            }
        }

        public void OnProductSerialButtonGroupClick(string productSerial)
        {
            SetProductSerialAsCurrent(productSerial);
            //StateHasChanged();
        }

        // public void OnTSheetInspectionCompleted(string productSerial)
        // {
        //     var tSheetSpec = ProductSerialToTSheetSpecDict[productSerial];
        //     ProductSerialToTSheetSpecDict[productSerial].IsInspectionCompleted = true;
        // 
        //     TSheetSpecsInCloseClientService.DeleteTSheetSpecsInCloseByUserIdProductSerial(productSerial);
        // 
        //     ProductSerials.Remove(productSerial);
        //     ProductSerialsSelected.Remove(productSerial);
        //     ProductSerialToTSheetSpecDict.Remove(productSerial);
        // 
        //     var firstEntry = ProductSerialToTSheetSpecDict.FirstOrDefault();
        //     if (firstEntry.Key != null)
        //     {
        //         SetProductSerialAsCurrent(firstEntry.Key);
        //     }
        //     else
        //     {
        //         // Products empty, so no display of TSheetComponent
        //         CurrentTSheetSpec = null;
        //     }
        // 
        //     StateHasChanged();
        // }

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

        //private async Task<bool> AddTProduct(TProductSpec tProductSpec)
        //{
        //    //AddNewProductDialogVisible = false;

        //    if (tProductSpec.ProductModel == "MMMMMMMM")
        //    {
        //        // invalid ProductModel
        //        return false;
        //    }

        //    if (ProductSerialToTSheetSpecDict.ContainsKey(tProductSpec.ProductSerial))
        //    {
        //        //already added
        //        await ActivateAlert("추가 실패", "이미 등록되었습니다.");
        //        return false;
        //    }

        //    //GridData = ProductService.GetProducts().ToList();
        //    TSheetSpec tSheetSpec = await GetTSheetSpecByTModelAsync(tProductSpec.ProductModel);
        //    if (tSheetSpec != null)
        //    {
        //        tSheetSpec.ProductSerial = tProductSpec.ProductSerial;
        //        tSheetSpec.ProductModel = tProductSpec.ProductModel;
        //        tSheetSpec.Customer = tProductSpec.Customer;
        //        tSheetSpec.EndUser = tProductSpec.EndUser;
        //        tSheetSpec.ProductType = tProductSpec.ProductType;
        //        tProductSpec.TSheetSpec = tSheetSpec;

        //        ProductSerials ??= new List<string>();
        //        ProductSerials.Add(tProductSpec.ProductSerial);

        //        ProductSerialToTSheetSpecDict.Add(tProductSpec.ProductSerial, tSheetSpec);
        //        ProductSerialsSelected.Add(tProductSpec.ProductSerial, false);

        //        SetProductSerialAsCurrent(tProductSpec.ProductSerial);
        //        //StateHasChanged();
        //        return true;
        //    }
        //    else
        //    {
        //        //await ActivateAlert("WARNING", $"{tProductSpec.ProductModel}에 대한 스펙파일을 찾을 수 없습니다. 서버를 점검하세요.");
        //        await ActivateAlert("추가 실패", "서버연결상태를 점검하세요.");
        //        return false;
        //    }
        //}

        private void SetProductSerialAsCurrent(string productSerial)
        {
            CurrentTSheetSpec = ProductSerialToTSheetSpecDict[productSerial] as TSheetSpec;

            ProductSerialsSelected.Keys.ToList().ForEach(serial =>{ProductSerialsSelected[serial] = false;});
            ProductSerialsSelected[productSerial] = true;

            StateHasChanged();
        }

        private async Task<TSheetSpec> GetTSheetSpecByTModelAsync(string tModel)
        {
            TSheetSpec tSheetSpec = await TSheetSpecsClientService.GetTSheetSpecByEquipmentModelAsync(tModel);

            if (tSheetSpec != null)
            {
                if (tSheetSpec.ProductModel != Constants.TSheetSpecNotDefined)
                {
                    ExpandTSheetSpec(ref tSheetSpec); //call by ref
                    MakeRangeToolTip(ref tSheetSpec); //call by ref
                    CreateDirtyFields(ref tSheetSpec); //call by ref
                    SetChXEnabled(ref tSheetSpec); //call by ref
                    MakeCategoryObservableTItemSpecsDict(ref tSheetSpec);
                    MakeTItemSpecsCompletedCountInCategoryDict(ref tSheetSpec);
                    MakeTItemSpecsInCategoryInvalidCountDict(ref tSheetSpec);
                    CleanUpTSheetSpec(ref tSheetSpec);
                }
            }

            return tSheetSpec;
        }

        #region TSheetSpec Manipulation
        private void ExpandTSheetSpec(ref TSheetSpec tSheetSpecRef) //call by ref
        {
            var tItemSpecs = tSheetSpecRef.TItemSpecs;

            //Extract unique Category
            tSheetSpecRef.TCategories = (List<string>)tItemSpecs.GroupBy(s => s.Category).Select(s => s.First()).Select(g => g.Category).ToList();

            //Group TItemSpecs by Category, Put into a Dictionary.
            tSheetSpecRef.TItemSpecsInCategoryDict = tItemSpecs.GroupBy(s => s.Category).ToDictionary(g => g.Key, g => g.ToList());

            tSheetSpecRef.InspectionStartDateTime = DateTime.Now;
            tSheetSpecRef.InspectorName = CurrentInspectorName;

            tSheetSpecRef.IsInInspecting = false;
            tSheetSpecRef.IsInClosing = true;
        }

        private void MakeCategoryObservableTItemSpecsDict(ref TSheetSpec tSheetSpecRef)
        {
            string model = tSheetSpecRef.ProductModel;
            //if (tSheetSpecRef.TCategories.IsNullOrEmpty())
            //    tSheetSpecRef.TCategories = new List<string>();
            //else
            //    tSheetSpecRef.TCategories.Clear();

            if (tSheetSpecRef.ObservableTItemSpecsInCategoryDict.IsNullOrEmpty())
                tSheetSpecRef.ObservableTItemSpecsInCategoryDict = new Dictionary<string, ObservableCollection<TItemSpec>>();
            else
                tSheetSpecRef.ObservableTItemSpecsInCategoryDict.Clear();

            if (tSheetSpecRef.TItemsCountInCategoryDict.IsNullOrEmpty())
                tSheetSpecRef.TItemsCountInCategoryDict = new Dictionary<string, int>();
            else
                tSheetSpecRef.TItemsCountInCategoryDict.Clear();

            foreach (var categoryTItemspec in tSheetSpecRef.TItemSpecsInCategoryDict)
            {
                //tSheetSpecRef.TCategories.Add(categoryTItemspec.Key);
                ObservableCollection<TItemSpec> observableTItemSpecs = new ObservableCollection<TItemSpec>(categoryTItemspec.Value);
                tSheetSpecRef.ObservableTItemSpecsInCategoryDict.Add(categoryTItemspec.Key, observableTItemSpecs);
                tSheetSpecRef.TItemsCountInCategoryDict.Add(categoryTItemspec.Key, observableTItemSpecs.Count);
            }
        }

        private void MakeTItemSpecsCompletedCountInCategoryDict(ref TSheetSpec tSheetSpecRef)
        {
            if (tSheetSpecRef.TItemSpecsCompletedCountInCategoryDict.IsNullOrEmpty())
                tSheetSpecRef.TItemSpecsCompletedCountInCategoryDict = new Dictionary<string, int>();
            else
                tSheetSpecRef.TItemSpecsCompletedCountInCategoryDict.Clear();
        }

        private void MakeTItemSpecsInCategoryInvalidCountDict(ref TSheetSpec tSheetSpecRef)
        {
            if (tSheetSpecRef.TItemSpecsInvalidCountInCategoryDict.IsNullOrEmpty())
                tSheetSpecRef.TItemSpecsInvalidCountInCategoryDict = new Dictionary<string, int>();
            else
                tSheetSpecRef.TItemSpecsInvalidCountInCategoryDict.Clear();
        }

        private void MakeRangeToolTip(ref TSheetSpec tSheetSpecRef) //call by ref
        {
            foreach (var tItemSpec in tSheetSpecRef.TItemSpecs)
            {
                if (tItemSpec.ExpressionMode.Contains("Combo"))
                {
                    var comboList = tItemSpec.Unit.Split(',').ToList();
                    tItemSpec.UnitList = comboList.Select(t => t.Trim()).ToList();

                    if (tItemSpec.Channels == 1)
                    {
                        tItemSpec.RangeToolTip = $"CH1: {tItemSpec.Unit}";
                    }
                    else if (tItemSpec.Channels == 2)
                    {
                        tItemSpec.RangeToolTip = $"CH1|CH2: {tItemSpec.Unit}";
                    }
                    else if (tItemSpec.Channels == 3)
                    {
                        tItemSpec.RangeToolTip = $"CH1|CH2|CH3: {tItemSpec.Unit}";
                    }
                    else if (tItemSpec.Channels == 4)
                    {
                        tItemSpec.RangeToolTip = $"CH1|CH2|CH3|CH4: {tItemSpec.Unit}";
                    }
                    else //something wrong
                    {
                        tItemSpec.RangeToolTip = $"ERROR: Too Many Channels!";
                    }
                }
                else
                {
                    if (tItemSpec.Channels == 1)
                    {
                        tItemSpec.RangeToolTip = $"CH1: {tItemSpec.Ch1LCL}~{tItemSpec.Ch1UCL}";
                    }
                    else if (tItemSpec.Channels == 2)
                    {
                        tItemSpec.RangeToolTip = $"CH1: {tItemSpec.Ch1LCL}~{tItemSpec.Ch1UCL},    CH2: {tItemSpec.Ch2LCL}~{tItemSpec.Ch2UCL}";
                    }
                    else if (tItemSpec.Channels == 3)
                    {
                        tItemSpec.RangeToolTip = $"CH1: {tItemSpec.Ch1LCL}~{tItemSpec.Ch1UCL},    CH2: {tItemSpec.Ch2LCL}~{tItemSpec.Ch2UCL},    CH3: {tItemSpec.Ch3LCL}~{tItemSpec.Ch3UCL}";
                    }
                    else if (tItemSpec.Channels == 4)
                    {
                        tItemSpec.RangeToolTip = $"CH1: {tItemSpec.Ch1LCL}~{tItemSpec.Ch1UCL},    CH2: {tItemSpec.Ch2LCL}~{tItemSpec.Ch2UCL},    CH3: {tItemSpec.Ch3LCL}~{tItemSpec.Ch3UCL},    CH4: {tItemSpec.Ch4LCL}~{tItemSpec.Ch4UCL}";
                    }
                    else //something wrong
                    {
                        tItemSpec.RangeToolTip = $"ERROR: Too Many Channels!";
                    }
                }
            }
        }

        private void CreateDirtyFields(ref TSheetSpec tSheetSpecRef) //call by ref
        {
            foreach (var tItemSpec in tSheetSpecRef.TItemSpecs)
            {
                tItemSpec.DirtyFields = new List<string>();
            }
        }

        private void SetChXEnabled(ref TSheetSpec tSheetSpecRef) //call by ref
        {
            foreach (var tItemSpec in tSheetSpecRef.TItemSpecs)
            {
                if (tItemSpec.Channels >= 4)
                    tItemSpec.IsCh4DataEnabled = true;
                else
                    tItemSpec.IsCh4DataEnabled = false;

                if (tItemSpec.Channels >= 3)
                    tItemSpec.IsCh3DataEnabled = true;
                else
                    tItemSpec.IsCh3DataEnabled = false;

                if (tItemSpec.Channels >= 2)
                    tItemSpec.IsCh2DataEnabled = true;
                else
                    tItemSpec.IsCh2DataEnabled = false;

                if (tItemSpec.Channels >= 1)
                    tItemSpec.IsCh1DataEnabled = true;
                else
                    tItemSpec.IsCh1DataEnabled = false;
            }
        }

        //private void OnCellRender(GridCellRenderEventArgs args)
        //{
        //    args.Class = "center-cell";
        //}

        private void CleanUpTSheetSpec(ref TSheetSpec tSheetSpecRef) //call by ref
        {
            tSheetSpecRef.TItemSpecsInCategoryDict.Clear();
            tSheetSpecRef.TItemSpecs.Clear();
        }
        #endregion


        public async void OnLoadSessionData()
        {
            IsLoadingSession = true;

            var state = await this.AuthStateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            //var authState = await AuthenticationStateTask;
            //var user = authState.User;
            CurrentInspectorName = user.GetHangulName();
            CurrentInspectorUserId = user.GetUserId();

            TSheetSpecsInProgressDto tSheetSpecsInCloseDto = await TSheetSpecsInCloseClientService.GetTSheetSpecsInCloseByUser(CurrentInspectorUserId);
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
                    tSheetSpec.IsInInspecting = false;
                    tSheetSpec.IsInClosing = true;

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

            CurrentInspectorName = user.GetHangulName();
            CurrentInspectorUserId = user.GetUserId();

            TSheetSpecsInProgressDto tSheetSpecsInCloseReqeust = new TSheetSpecsInProgressDto
            {
                UserId = CurrentInspectorUserId,
                SerialToTSheetSpecPairs = new Dictionary<string, string>()
            };

            int countInProgress = 0;

            foreach (var productSerialToTSheetSpec in ProductSerialToTSheetSpecDict)
            {
                var serial = productSerialToTSheetSpec.Key;
                var tSheetSpec = productSerialToTSheetSpec.Value;
                if ( !tSheetSpec.IsInspectionCompleted )
                {
                    // save "In-Progress" inspections only. do not save "Completed" inspections
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
