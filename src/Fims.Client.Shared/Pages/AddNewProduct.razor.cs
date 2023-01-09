using AutoMapper;
using Fims.Client.Shared.ClientModels;
using Fims.Client.Shared.ClientServices.TSheetSpecs;
using Fims.Common;
using Fims.Common.Mapping;
using Fims.Data.Entities;
using Fims.Data.Models;
using Fims.Data.Models.TSheetSpecs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Telerik.Blazor;
using Telerik.Blazor.Components;
using Telerik.DataSource;
using static System.Net.Mime.MediaTypeNames;
using static Telerik.Blazor.ThemeConstants;


namespace Fims.Client.Shared.Pages
{
    public partial class AddNewProduct
    {

        public string ProductSerial { get; set; }
        public string ProductModel { get; set; }
        public string Customer { get; set; }
        public string EndUser { get; set; }
        protected string ProductType { get; set; } = "신규";

        private List<string> ProductModels { get; set; } = new List<string>();
        protected List<string> ProductTypes = new List<string>() { "신규", "수리" };

        private bool ManualSelectionDialogVisible { get; set; } = false;
        private bool BarcodeSelectionDialogVisible { get; set; } = false;
        private bool ProgressListDialogVisible { get; set; } = false;


        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            var state = await this.AuthState.GetAuthenticationStateAsync();
            var user = state.User;

            ProductModels = await TSheetSpecsClientService.GetEquipmentModelsAsync();
        }

        private void OnManualSelectionClicked()
        {
            ManualSelectionDialogVisible = true;
        }

        private void OnBarcodeSelectionClicked()
        {
            BarcodeSelectionDialogVisible = true;
        }

        private void OnProgessListClicked()
        {
            ProgressListDialogVisible = true;
        }


        private void OnAddNewProductOK()
        {
            ManualSelectionDialogVisible = false;

            var product = new Dictionary<string, string>()
            {
                {"ProductSerial", ProductSerial},
                {"ProductModel", ProductModel},
                {"Customer", Customer},
                {"EndUser", EndUser},
                {"ProductType", ProductType},

            };

            //var query = new Dictionary<string, string> { { "name", "Mike" } };
            NavigationManager.NavigateTo(QueryHelpers.AddQueryString("/", product), forceLoad: false);
        }

        private void OnAddNewProductCancel()
        {
            ManualSelectionDialogVisible = false;
            NavigationManager.NavigateTo("/");
        }


        /// <summary>
        /// Popup Dialog
        /// </summary>
        private void OnSelectionDialogOK()
        {
            ManualSelectionDialogVisible = false;
        }

        private void OnSelectionDialogCancel()
        {
            ManualSelectionDialogVisible = false;
        }

        public void OnProductSerialButtonGroupClick(string productSerial)
        {
            // foreach (var serialsel in ProductSerialToSelectionDict)
            // {
            //     var key = serialsel.Key;
            //     var val = serialsel.Value;
            // }

            //SetProductSerialAsCurrent(productSerial);
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
