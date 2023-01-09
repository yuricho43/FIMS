using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
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

namespace Fims.Client.Shared.Pages
{
    public partial class Index
    {
        #region PassByQueryStrings
        [Parameter]
        [SupplyParameterFromQuery(Name = "ProductSerial")]
        public string? XXProductSerial { get; set; }

        [Parameter]
        [SupplyParameterFromQuery(Name = "ProductModel")]
        public string? XXProductModel { get; set; }
 
        [Parameter]
        [SupplyParameterFromQuery(Name = "Customer")]
        public string? XXCustomer { get; set; }

        [Parameter]
        [SupplyParameterFromQuery(Name = "EndUser")]
        public string? XXEndUser { get; set; }
 
        [Parameter]
        [SupplyParameterFromQuery(Name = "ProductType")]
        public string? XXProductType { get; set; }

        // For String List such as "/querystrings?filter=scifi stars&page=3&star=LeVar Burton&star=Gary Oldman"
        //      [Parameter]
        //      [SupplyParameterFromQuery]
        //      public string? Filter { get; set; }
        //      
        //      [Parameter]
        //      [SupplyParameterFromQuery]
        //      public int? Page { get; set; }
        //      
        //      [Parameter]
        //      [SupplyParameterFromQuery(Name = "star")]
        //      public string[]? Stars { get; set; }
        #endregion

        private List<string>                   ProductSerials { get; set; } = new List<string>();
        private Dictionary<string, TSheetSpec> ProductSerialToTSheetSpecDict { get; set; } = new Dictionary<string, TSheetSpec>();
        private Dictionary<string, bool>       ProductSerialToSelectionDict { get; set; } = new Dictionary<string, bool>();

        private List<string> ProductModels { get; set; } = new List<string>();

        private string     CurrentProductSerial { get; set; }
        private TSheetSpec CurrentTSheetSpec { get; set; }
        private string     CurrentInspectorName { get; set; }

        public bool AddNewProductDialogVisible { get; set; } = false;

        public int Page { get; set; } = 1;

        TelerikNotification IndexNotificationComponent { get; set; }


        protected override void OnInitialized()
        {
            Layout.DocsTitle = Localizer["HumanCapital"];

            base.OnInitialized();
        }

        //FIXME  protected override async Task OnInitializedAsync()
        //FIXME  {
        //FIXME      // Accessing LocalStorage at this phase is not allowed. JSRuntime out of WebView.
        //FIXME      // So do it after rendering finished.
        //FIXME
        //FIXME      var state = await this.AuthState.GetAuthenticationStateAsync();
        //FIXME      var user = state.User;
        //FIXME      CurrentInspectorName = user.GetFirstName();
        //FIXME  
        //FIXME      ProductModels = await TSheetSpecsClientService.GetEquipmentModelsAsync();
        //FIXME  }

        protected override async Task OnParametersSetAsync()
        {
            if (XXProductSerial.IsNullOrEmpty())
            { 
                return;
            }

            var product = new TProductSpec
            {
                ProductSerial = XXProductSerial,
                ProductModel = XXProductModel,
                Customer = XXCustomer,
                EndUser = XXEndUser,
                ProductType = XXProductType,
            };

            bool result = await AddTProduct(product);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            // Accessing LocalStorage at the initializing phase is not allowed. JSRuntime out of WebView.
            // So do it here after rendering finished.
            if (firstRender)
            {
                var state = await this.AuthState.GetAuthenticationStateAsync();
                var user = state.User;
                CurrentInspectorName = user.GetFirstName();

                ProductModels = await TSheetSpecsClientService.GetEquipmentModelsAsync();
            }
        }

        public void OnProductSerialButtonGroupClick(string productSerial)
        {
            // foreach (var serialsel in ProductSerialToSelectionDict)
            // {
            //     var key = serialsel.Key;
            //     var val = serialsel.Value;
            // }

            SetProductSerialAsCurrent(productSerial);
        }

        private async Task<bool> AddTProduct(TProductSpec tProductSpec)
        {
            AddNewProductDialogVisible = false;

            if (tProductSpec.ProductModel == "MMMMMMMM")
            {
                return false; // invalid ProductModel
            }

            if (ProductSerialToTSheetSpecDict.ContainsKey(tProductSpec.ProductSerial))
            {
                return false;
            }

            //GridData = ProductService.GetProducts().ToList();
            TSheetSpec tSheetSpec = await GetTSheetSpecByTModelAsync(tProductSpec.ProductModel);
            if (tSheetSpec != null)
            {
                tSheetSpec.ProductSerial = tProductSpec.ProductSerial;
                tSheetSpec.ProductModel = tProductSpec.ProductModel;
                tSheetSpec.Customer = tProductSpec.Customer;
                tSheetSpec.EndUser = tProductSpec.EndUser;
                tSheetSpec.ProductType = tProductSpec.ProductType;
                tProductSpec.TSheetSpec = tSheetSpec;

                ProductSerials ??= new List<string>();
                ProductSerials.Add(tProductSpec.ProductSerial);

                ProductSerialToTSheetSpecDict.Add(tProductSpec.ProductSerial, tSheetSpec);
                ProductSerialToSelectionDict.Add(tProductSpec.ProductSerial, false);

                SetProductSerialAsCurrent(tProductSpec.ProductSerial);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SetProductSerialAsCurrent(string productSerial)
        {
            CurrentTSheetSpec = ProductSerialToTSheetSpecDict[productSerial] as TSheetSpec;
            CurrentProductSerial = productSerial;
            ProductSerialToSelectionDict[productSerial] = true; //??
        }

        private async Task<TSheetSpec> GetTSheetSpecByTModelAsync(string tModel)
        {
            TSheetSpec tSheetSpec;
            //FIXME  if (TModelToTSheetSpecDict.ContainsKey(tModel))
            //FIXME  {
            //FIXME      tSheetSpec = TModelToTSheetSpecDict[tModel];
            //FIXME  }
            //FIXME  else
            //FIXME  {
                tSheetSpec = await TSheetSpecsClientService.GetTSheetSpecByEquipmentModelAsync(tModel);
                if (tSheetSpec.ProductModel != Constants.TSheetSpecNotDefined)
                {
                    ExpandTSheetSpec(ref tSheetSpec); //call by ref
                    MakeRangeToolTip(ref tSheetSpec); //call by ref
                    CreateDirtyFields(ref tSheetSpec); //call by ref
                    SetChXEnabled(ref tSheetSpec); //call by ref
                    MakeCategoryObservableTItemSpecsDict(ref tSheetSpec);
                }
                else
                {
                    await ActivateAlert("WARNING", $"Test Spec Not Found for Model: {tModel}");
                }
            //FIXME  }

            var tmodel = tSheetSpec.ProductModel;
            var tcounts = tSheetSpec.CategoryTItemsCountDict.Values.ToList();
            return tSheetSpec;
        }


        private void ExpandTSheetSpec(ref TSheetSpec tSheetSpecRef) //call by ref
        {
            var tItemSpecs = tSheetSpecRef.TItemSpecs;

            //Extract unique Category
            tSheetSpecRef.TCategories = (List<string>)tItemSpecs.GroupBy(s => s.Category).Select(s => s.First()).Select(g => g.Category).ToList();

            //Group TItemSpecs by Category, Put into a Dictionary.
            tSheetSpecRef.TCategoryToTItemSpecsDict = tItemSpecs.GroupBy(s => s.Category).ToDictionary(g => g.Key, g => g.ToList());

            tSheetSpecRef.InspectionStartDateTime = DateTime.Now;
            tSheetSpecRef.InspectorName = CurrentInspectorName;
        }

        private void MakeCategoryObservableTItemSpecsDict(ref TSheetSpec tSheetSpecRef)
        {
            string model = tSheetSpecRef.ProductModel;
            //if (tSheetSpecRef.TCategories.IsNullOrEmpty())
            //    tSheetSpecRef.TCategories = new List<string>();
            //else
            //    tSheetSpecRef.TCategories.Clear();

            if (tSheetSpecRef.TCategoryToObservableTItemSpecsDict.IsNullOrEmpty())
                tSheetSpecRef.TCategoryToObservableTItemSpecsDict = new Dictionary<string, ObservableCollection<TItemSpec>>();
            else
                tSheetSpecRef.TCategoryToObservableTItemSpecsDict.Clear();

            if (tSheetSpecRef.CategoryTItemsCountDict.IsNullOrEmpty())
                tSheetSpecRef.CategoryTItemsCountDict = new Dictionary<string, int>();
            else
                tSheetSpecRef.CategoryTItemsCountDict.Clear();

            foreach (var categoryTItemspec in tSheetSpecRef.TCategoryToTItemSpecsDict)
            {
                //tSheetSpecRef.TCategories.Add(categoryTItemspec.Key);
                ObservableCollection<TItemSpec> observableTItemSpecs = new ObservableCollection<TItemSpec>(categoryTItemspec.Value);
                tSheetSpecRef.TCategoryToObservableTItemSpecsDict.Add(categoryTItemspec.Key, observableTItemSpecs);
                tSheetSpecRef.CategoryTItemsCountDict.Add(categoryTItemspec.Key, observableTItemSpecs.Count);
            }
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


        public void OnAddNewProductClicked()
        {
            AddNewProductDialogVisible = true;
            //NavigationManager.NavigateTo("/AddNewProduct");
            //StateHasChanged();
        }

        public async void OnLoadSessionData()
        {
            string userId = "coolbix";
            Stream jsonStream = await TSheetSpecsInProgressClientService.GetTSheetSpecsInProgressByUser(userId);

            Dictionary<string, TSheetSpec> productSerialToTSheetSpecDict = await JsonSerializer.DeserializeAsync<Dictionary<string, TSheetSpec>>(jsonStream);

            foreach (var productSerialToTSheetSpec in productSerialToTSheetSpecDict)
            {
                if ( !ProductSerialToTSheetSpecDict.ContainsKey(productSerialToTSheetSpec.Key) )
                {
                    var key = productSerialToTSheetSpec.Key;
                    var value = productSerialToTSheetSpec.Value;
                    ProductSerialToTSheetSpecDict?.Add(productSerialToTSheetSpec.Key, productSerialToTSheetSpec.Value);
                    ProductSerialToSelectionDict?.Add(productSerialToTSheetSpec.Key, false);
                    ProductSerials?.Add(productSerialToTSheetSpec.Key);

                }
            }

            //SetProductSerialAsCurrent(tProductSpec.ProductSerial);

            StateHasChanged();

            IndexNotificationComponent.Show(new NotificationModel()
            {
                Text = "진행목록이 성공적으로 로딩되었습니다.",
                ThemeColor = "info",
                ShowIcon = true,
                Icon = "caret-double-alt-down"
            });
        }

        public async void OnSaveSessionData()
        {
            var jsonString = JsonUtils.PrettySerialize(ProductSerialToTSheetSpecDict);
            TSheetSpecsInProgressReqeust tSheetSpecsInProgressReqeust = new TSheetSpecsInProgressReqeust
            {
                UserId = "coolbix",
                TSheetSpecsJsonString = jsonString
            };
            var fileName = await TSheetSpecsInProgressClientService.SaveTSheetSpecsInProgressByUser(tSheetSpecsInProgressReqeust);

            IndexNotificationComponent.Show(new NotificationModel()
            {
                Text = "진행목록이 성공적으로 저장되었습니다.",
                ThemeColor = "success",
                ShowIcon = true,
                Icon = "caret-double-alt-up"
            });

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


    public static class EnumerableExtensions
    {
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? source) => source ?? Enumerable.Empty<T>();
    }
}
