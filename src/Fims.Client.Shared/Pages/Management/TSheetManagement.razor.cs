using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;

using Telerik.Blazor;
using Telerik.Blazor.Components;
using Telerik.DataSource;

using AutoMapper;

using Fims.Client.Shared.ClientServices.TReports;
using Fims.Common;
using Fims.Data.Entities;
using Fims.Data.Models;
using Fims.Data.Models.TReports;

namespace Fims.Client.Shared.Pages.Management
{
    public partial class TSheetManagement
    {
        TelerikGrid<TSheet> TSheetManagementGridRef { get; set; }

        public IEnumerable<TSheet> TSheets { get; set; } = Enumerable.Empty<TSheet>();
        public IEnumerable<TSheet> SelectedTSheets { get; set; } = Enumerable.Empty<TSheet>();


        public List<string> TReportSpecs { get; set; } = new List<string>();
        public TReportDto TReportGenerated { get; set; } = new TReportDto();


        int filterDebounceDelay { get; set; } = 200;
        int PageSize = 10;


        public string TextBoxFillMode { get; set; } = ThemeConstants.TextBox.FillMode.Solid;
        public string TextBoxRounded { get; set; } = ThemeConstants.TextBox.Rounded.Medium;
        public string TextBoxSize { get; set; } = ThemeConstants.TextBox.Size.Medium;


        protected override void OnInitialized()
        {
            //TSheets = productService.GetProducts();
        }

        protected override async Task OnInitializedAsync()
        {
            TSheets = await TSheetsClientService.AllTSheetsAsync();
        }

        public void ShowDetailsHandler(GridCommandEventArgs args)
        {
            var tSheet = (TSheet)args.Item;
            this.NavigationManager.NavigateTo($"/Management/TSheetDetails/{tSheet.Id}", forceLoad: true);
        }


        public async void GetTReportSpecsHandler(GridCommandEventArgs args)
        {
            TReportSpecs = await TReportsClientService.AllTReportSpecs();
        }

        public async void GenerateReportHandler(GridCommandEventArgs args)
        {
            var tSheet = (TSheet)args.Item;
            TReportDto reportRequest = new TReportDto
            {
                TSheetId = tSheet.Id,
                TReportTemplateFile = "FimsTReportSpecs_CHILLER 검사 성적서_20221226.xlsx",
                TReportOutputFile = "FimsReport_CHILLER 검사 성적서_20221226.xlsx",
                IsSuccess = false,
            };

            TReportGenerated  = await TReportsClientService.GenerateTReport(reportRequest);
        }

        public void DeleteTSheetHandler(GridCommandEventArgs args)
        {
            // ProductService.DeleteProduct((ProductDto)args.Item);
            // LoadData();
        }


        public List<FilterOperator> filterOperators { get; set; } = new List<FilterOperator>()
        {
            FilterOperator.IsEqualTo,
            FilterOperator.IsNotEqualTo,
            FilterOperator.StartsWith,
            FilterOperator.Contains,
            FilterOperator.DoesNotContain
        };

        public List<FilterListOperator> NumericOperators { get; set; } = new List<FilterListOperator>()
        {
            new FilterListOperator { Operator = FilterOperator.IsEqualTo, Text = "Equals" },
            new FilterListOperator { Operator = FilterOperator.IsLessThan, Text = "Smaller than" },
            new FilterListOperator { Operator = FilterOperator.IsGreaterThan, Text = "Larger than" },
            new FilterListOperator { Operator = FilterOperator.IsLessThanOrEqualTo, Text = "Smaller than or equals" },
            new FilterListOperator { Operator = FilterOperator.IsGreaterThanOrEqualTo, Text = "Larger than or equals" }
        };

        private void OnFilterOperatorChanged()
        {
            // var state = TSheetManagementGridRef?.GetState();
            // 
            // var productNameFilter = state.FilterDescriptors.FirstOrDefault(f => (f as FilterDescriptor).Member == nameof(TSheet.ProductModel));
            // 
            // if (productNameFilter != null)
            // {
            //     state.FilterDescriptors.Remove(productNameFilter);
            // 
            //     TSheetManagementGridRef?.SetState(state);
            // }
        }


        private async Task ExportToExcel()
        {
            await Task.Delay(10); // 10ms
        }


        [CascadingParameter]
        public DialogFactory Dialogs { get; set; }
        public async Task ActivateAlert(string title, string message)
        {
            if (string.IsNullOrWhiteSpace(title)) title = "Warning!";
            if (string.IsNullOrWhiteSpace(message)) message = "Something went wrong!";

            await Dialogs.AlertAsync(message, title);
        }


/*
        #region TextBoxOnChangeHandlers
        private void TextBoxOnChangeHandler1(object theUserInput, TItemSpec itemspec)
        {
            if (itemspec.ExpressionMode == "Number")
            {
                bool needToCheckLcl = true;
                bool needToCheckUcl = true;

                if (Decimal.TryParse(theUserInput as string, out decimal inputval))
                {
                    if (!Decimal.TryParse(itemspec.Ch1LCL, out decimal lcl))
                        needToCheckLcl = false; //LCL is not defined properly, so skip the check.

                    if (!Decimal.TryParse(itemspec.Ch1UCL, out decimal ucl))
                        needToCheckUcl = false; //UCL is not defined properly, so skip the check.

                    if ((needToCheckLcl) && (inputval < lcl))
                    {
                        MarkCh1DataInvalid(itemspec);
                    }
                    else
                    {
                        MarkCh1DataValid(itemspec);
                    }

                    if ((needToCheckUcl) && (inputval > ucl))
                    {
                        MarkCh1DataInvalid(itemspec);
                    }
                    else
                    {
                        MarkCh1DataValid(itemspec);
                    }
                }
                else
                {
                    //UserInput is not a number string
                    MarkCh1DataInvalid(itemspec);
                }
            }
            else
            {
                //String, String/Combo
                if (string.IsNullOrEmpty(theUserInput as string))
                {
                    MarkCh1DataInvalid(itemspec);
                }
                else
                {
                    MarkCh1DataValid(itemspec);
                }
            }
        }

        private void MarkCh1DataValid(TItemSpec itemspec)
        {
            itemspec.IsCh1DataValid = true;
            //itemspec.IsCh1DataEntered = true;
        }

        private void MarkCh1DataInvalid(TItemSpec itemspec)
        {
            //itemspec.Ch1Data = "대한민국";
            itemspec.IsCh1DataValid = false;
            //itemspec.IsCh1DataEntered = true;

            //TextBoxFillMode = ThemeConstants.TextBox.FillMode.Outline;
            //TextBoxRounded = ThemeConstants.TextBox.Rounded.Full;
            //update the UI
            //JBH FIXME: Really Needed?    StateHasChanged();
        }

        private void TextBoxOnChangeHandler2(object theUserInput, TItemSpec itemspec)
        {
            // the handler receives an object that you may need to cast
            string result = string.Format("The user entered: {0}", theUserInput);

            //var myTItemSpec = MyTSheetSpec.TItemSpecs.Single<TItemSpec>(x => x.TestNo == itemspec.TestNo);
            var cool = itemspec.Ch2LCL;
            itemspec.Ch2LCL = "위대한";
            //var one = itemspec.Ch2LCL;
            //var two = myTItemSpec.Ch2LCL;

            TextBoxFillMode = ThemeConstants.TextBox.FillMode.Outline;
            TextBoxRounded = ThemeConstants.TextBox.Rounded.Full;

            //update the UI
            //JBH FIXME: Really Needed?    StateHasChanged();
        }

        private void TextBoxOnChangeHandler3(object theUserInput, TItemSpec itemspec)
        {
            // the handler receives an object that you may need to cast
            string result = string.Format("The user entered: {0}", theUserInput);

            //var myTItemSpec = MyTSheetSpec.TItemSpecs.Single<TItemSpec>(x => x.TestNo == itemspec.TestNo);
            var cool = itemspec.Ch3LCL;
            itemspec.Ch3LCL = "자유우선";
            //var one = itemspec.Ch3LCL;
            //var two = myTItemSpec.Ch3LCL;

            TextBoxFillMode = ThemeConstants.TextBox.FillMode.Outline;
            TextBoxRounded = ThemeConstants.TextBox.Rounded.Full;

            //update the UI
            //JBH FIXME: Really Needed?    StateHasChanged();
        }

        private void TextBoxOnChangeHandler4(object theUserInput, TItemSpec itemspec)
        {
            // the handler receives an object that you may need to cast
            string result = string.Format("The user entered: {0}", theUserInput);

            //var myTItemSpec = MyTSheetSpec.TItemSpecs.Single<TItemSpec>(x => x.TestNo == itemspec.TestNo);
            var cool = itemspec.Ch4LCL;
            itemspec.Ch4LCL = "민주국가";
            //var one = itemspec.Ch4LCL;
            //var two = myTItemSpec.Ch4LCL;

            TextBoxFillMode = ThemeConstants.TextBox.FillMode.Outline;
            TextBoxRounded = ThemeConstants.TextBox.Rounded.Full;

            //update the UI
            //JBH FIXME: Really Needed?    StateHasChanged();
        }

        private void TextBoxOnBlurHandler(object theUserInput)
        {
            // the handler receives an object that you may need to cast
            string result = string.Format("The user entered: {0}", theUserInput);
        }
        private void TextBoxValueChangedHandler(object theUserInput)
        {
            // the handler receives an object that you may need to cast
            string result = string.Format("The user entered: {0}", theUserInput);
        }
        #endregion


        #region Validations
        private bool ValidateUserInputCh1(string theUserInput, TItemSpec itemspec)
        {
            bool valid = false;
 
            if (itemspec.ExpressionMode == "Number")
            {
                bool needToCheckLcl = true;
                bool needToCheckUcl = true;

                if (Decimal.TryParse(theUserInput as string, out decimal inputval))
                {
                    if (!Decimal.TryParse(itemspec.Ch1LCL, out decimal lcl))
                        needToCheckLcl = false; //LCL is not defined properly, so skip the check.

                    if (!Decimal.TryParse(itemspec.Ch1UCL, out decimal ucl))
                        needToCheckUcl = false; //UCL is not defined properly, so skip the check.

                    if ((needToCheckLcl) && (inputval < lcl))
                    {
                        valid = false;
                    }
                    else
                    {
                        if ((needToCheckUcl) && (inputval > ucl))
                        {
                            valid = false;
                        }
                        else
                        {
                            valid = true;
                        }
                    }
                }
                else
                {
                    //UserInput is not a number string
                    valid = false;
                }
            }
            else
            {
                //String, String/Combo
                if (string.IsNullOrEmpty(theUserInput as string))
                {
                    valid = false;
                }
                else
                {
                    valid = true;
                }
            }

            return valid;
        }

        private bool ValidateUserInputCh2(string theUserInput, TItemSpec itemspec)
        {
            bool valid = false;

            if (itemspec.ExpressionMode == "Number")
            {
                bool needToCheckLcl = true;
                bool needToCheckUcl = true;

                if (Decimal.TryParse(theUserInput as string, out decimal inputval))
                {
                    if (!Decimal.TryParse(itemspec.Ch2LCL, out decimal lcl))
                        needToCheckLcl = false; //LCL is not defined properly, so skip the check.

                    if (!Decimal.TryParse(itemspec.Ch2UCL, out decimal ucl))
                        needToCheckUcl = false; //UCL is not defined properly, so skip the check.

                    if ((needToCheckLcl) && (inputval < lcl))
                    {
                        valid = false;
                    }
                    else
                    {
                        if ((needToCheckUcl) && (inputval > ucl))
                        {
                            valid = false;
                        }
                        else
                        {
                            valid = true;
                        }
                    }
                }
                else
                {
                    //UserInput is not a number string
                    valid = false;
                }
            }
            else
            {
                //String, String/Combo
                if (string.IsNullOrEmpty(theUserInput as string))
                {
                    valid = false;
                }
                else
                {
                    valid = true;
                }
            }

            return valid;
        }

        private bool ValidateUserInputCh3(string theUserInput, TItemSpec itemspec)
        {
            bool valid = false;

            if (itemspec.ExpressionMode == "Number")
            {
                bool needToCheckLcl = true;
                bool needToCheckUcl = true;

                if (Decimal.TryParse(theUserInput as string, out decimal inputval))
                {
                    if (!Decimal.TryParse(itemspec.Ch3LCL, out decimal lcl))
                        needToCheckLcl = false; //LCL is not defined properly, so skip the check.

                    if (!Decimal.TryParse(itemspec.Ch3UCL, out decimal ucl))
                        needToCheckUcl = false; //UCL is not defined properly, so skip the check.

                    if ((needToCheckLcl) && (inputval < lcl))
                    {
                        valid = false;
                    }
                    else
                    {
                        if ((needToCheckUcl) && (inputval > ucl))
                        {
                            valid = false;
                        }
                        else
                        {
                            valid = true;
                        }
                    }
                }
                else
                {
                    //UserInput is not a number string
                    valid = false;
                }
            }
            else
            {
                //String, String/Combo
                if (string.IsNullOrEmpty(theUserInput as string))
                {
                    valid = false;
                }
                else
                {
                    valid = true;
                }
            }

            return valid;
        }

        private bool ValidateUserInputCh4(string theUserInput, TItemSpec itemspec)
        {
            bool valid = false;

            if (itemspec.ExpressionMode == "Number")
            {
                bool needToCheckLcl = true;
                bool needToCheckUcl = true;

                if (Decimal.TryParse(theUserInput as string, out decimal inputval))
                {
                    if (!Decimal.TryParse(itemspec.Ch4LCL, out decimal lcl))
                        needToCheckLcl = false; //LCL is not defined properly, so skip the check.

                    if (!Decimal.TryParse(itemspec.Ch4UCL, out decimal ucl))
                        needToCheckUcl = false; //UCL is not defined properly, so skip the check.

                    if ((needToCheckLcl) && (inputval < lcl))
                    {
                        valid = false;
                    }
                    else
                    {
                        if ((needToCheckUcl) && (inputval > ucl))
                        {
                            valid = false;
                        }
                        else
                        {
                            valid = true;
                        }
                    }
                }
                else
                {
                    //UserInput is not a number string
                    valid = false;
                }
            }
            else
            {
                //String, String/Combo
                if (string.IsNullOrEmpty(theUserInput as string))
                {
                    valid = false;
                }
                else
                {
                    valid = true;
                }
            }

            return valid;
        }
        #endregion


        #region Grid CRUD events
        public void EditHandler(GridCommandEventArgs args)
        {
            TItemSpec item = (TItemSpec)args.Item;

            //prevent opening for edit for deleted items
            if (item.IsDeleted)
            {
                args.IsCancelled = true;
            }

            //show notification
        }

        public void UpdateHandler(GridCommandEventArgs args)
        {
            TItemSpec itemspec = args.Item as TItemSpec;
            string field = args.Field as string;
            string userinput = args.Value as string;

            if (userinput.IsNullOrEmpty())
            {
                //itemspec.IsCh1DataEntered = false;
                return;
            }

            if (field == "Ch1Data")
            {
                itemspec.IsCh1DataEntered = true;
                itemspec.IsCh1DataValid = ValidateUserInputCh1(userinput, itemspec);
            }

            if (field == "Ch2Data")
            {
                itemspec.IsCh2DataEntered = true;
                itemspec.IsCh2DataValid = ValidateUserInputCh2(userinput, itemspec);
            }

            if (field == "Ch3Data")
            {
                itemspec.IsCh3DataEntered = true;
                itemspec.IsCh3DataValid = ValidateUserInputCh3(userinput, itemspec);
            }

            if (field == "Ch4Data")
            {
                itemspec.IsCh4DataEntered = true;
                itemspec.IsCh4DataValid = ValidateUserInputCh4(userinput, itemspec);
            }


            if (!itemspec.IsDirty)
            {
                TItemSpec pristineItem = GetItemFromCollection(PristineItems, itemspec);
                if (pristineItem == null)
                {
                    //add only the first time a field is edited, later it is no longer pristine
                    PristineItems.Add(GetItemFromCollection(MyCategoryTItemSpecs, itemspec));
                }
            }

            itemspec.IsChanged = true;
            itemspec.DirtyFields.Add(args.Field);

            ChangeLocalItem(itemspec);
        }

        public void CreateHandler(GridCommandEventArgs args)
        {
            TItemSpec item = (TItemSpec)args.Item;
            item.TestNo = MyCategoryTItemSpecs.Max(model => model.TestNo) + 1;
            item.IsNew = true;
            MyCategoryTItemSpecs.Insert(0, item);
        }

        public void DeleteHandler(GridCommandEventArgs args)
        {
            TItemSpec item = (TItemSpec)args.Item;

            DeleteItem(item);

            //show notification for undelete
        }
        #endregion


        #region Grid Render Events
        public void OnRowRenderHandler(GridRowRenderEventArgs args)
        {
            var item = args.Item as TItemSpec;

            if (item.IsNew)
            {
                args.Class = "k-new-row";
            }

            if (item.IsDeleted)
            {
                args.Class = "k-deleted-row";
            }

            ////args.Class = item.IsCh1DataValid ? "" : "userinput-invalid";
            //if (item.IsCh1DataValid)
            //{
            //    args.Class = "";
            //}
            //else
            //{
            //    args.Class = "userinput-invalid";
            //}
        }

        public void OnCellRenderHandler(GridCellRenderEventArgs args, string field)
        {
            //args.Class = string.Empty;
            var itemspec = args.Item as TItemSpec;

            if (field == "Ch1Data")
            {
                if (!itemspec.IsCh1DataEnabled)
                {
                    args.Class = "k-disabled-cell";
                    return;
                }

                if (!itemspec.IsCh1DataEntered)
                {
                    //args.Class = string.Empty;
                    args.Class = "userinput-empty";
                    return;
                }

                if (!itemspec.IsCh1DataValid)
                {
                    args.Class = "userinput-invalid";
                    return;
                }
                else
                {
                    args.Class = "userinput-valid";
                    return;
                }
            }

            if (field == "Ch2Data")
            {
                if (!itemspec.IsCh2DataEnabled)
                {
                    args.Class = "k-disabled-cell";
                    return;
                }

                if (!itemspec.IsCh2DataEntered)
                {
                    //args.Class = string.Empty;
                    args.Class = "userinput-empty";
                    return;
                }

                if (!itemspec.IsCh2DataValid)
                {
                    args.Class = "userinput-invalid";
                    return;
                }
                else
                {
                    args.Class = "userinput-valid";
                    return;
                }
            }

            if (field == "Ch3Data")
            {
                if (!itemspec.IsCh3DataEnabled)
                {
                    args.Class = "k-disabled-cell";
                    return;
                }

                if (!itemspec.IsCh3DataEntered)
                {
                    //args.Class = string.Empty;
                    args.Class = "userinput-empty";
                    return;
                }

                if (!itemspec.IsCh3DataValid)
                {
                    args.Class = "userinput-invalid";
                    return;
                }
                else
                {
                    args.Class = "userinput-valid";
                    return;
                }
            }

            if (field == "Ch4Data")
            {
                if (!itemspec.IsCh4DataEnabled)
                {
                    args.Class = "k-disabled-cell";
                    return;
                }

                if (!itemspec.IsCh4DataEntered)
                {
                    //args.Class = string.Empty;
                    args.Class = "userinput-empty";
                    return;
                }

                if (!itemspec.IsCh4DataValid)
                {
                    args.Class = "userinput-invalid";
                    return;
                }
                else
                {
                    args.Class = "userinput-valid";
                    return;
                }
            }

            //if (((field == "Ch1Data") && (!itemspec.IsCh1DataEnabled)) ||
            //     ((field == "Ch2Data") && (!itemspec.IsCh2DataEnabled)) ||
            //     ((field == "Ch3Data") && (!itemspec.IsCh3DataEnabled)) ||
            //     ((field == "Ch4Data") && (!itemspec.IsCh4DataEnabled))
            //   )
            //{
            //    args.Class = "k-disabled-cell";
            //    return;
            //}

            //if (((field == "Ch1Data") && (!itemspec.IsCh1DataEntered)) ||
            //     ((field == "Ch2Data") && (!itemspec.IsCh2DataEntered)) ||
            //     ((field == "Ch3Data") && (!itemspec.IsCh3DataEntered)) ||
            //     ((field == "Ch4Data") && (!itemspec.IsCh4DataEntered))
            //   )
            //{
            //    args.Class = string.Empty;
            //    return;
            //}

            //if (((field == "Ch1Data") && (!itemspec.IsCh1DataValid)) ||
            //     ((field == "Ch2Data") && (!itemspec.IsCh2DataValid)) ||
            //     ((field == "Ch3Data") && (!itemspec.IsCh3DataValid)) ||
            //     ((field == "Ch4Data") && (!itemspec.IsCh4DataValid))
            //   )
            //{
            //    args.Class = "userinput-invalid";
            //}
            //else
            //{
            //    args.Class = "userinput-valid";
            //}


            //args.Class = !itemspec.IsNew && itemspec.DirtyFields.Contains(field) == true ? "k-changed-cell" : string.Empty;

            //args.Class = (bool)args.Value ? string.Empty : "userinput-valid";
            //if ((bool)args.Value)
            //{
            //    args.Class = string.Empty;
            //}
            //else
            //{
            //    args.Class = "userinput-valid";
            //}

            //if (itemspec.IsCh1DataValid)
            //{
            //    args.Class = string.Empty;
            //}
            //else
            //{
            //    args.Class = "userinput-invalid";
            //}
        }
        #endregion


        #region Grid Toolbar commands
        public void DeleteSelected()
        {
            foreach (TItemSpec item in SelectedItems)
            {
                DeleteItem(item);
            }

            SelectedItems = new List<TItemSpec>();
        }

        public void RevertSelected()
        {
            foreach (TItemSpec item in SelectedItems)
            {
                RevertItem(item);
            }

            SelectedItems = new List<TItemSpec>();
        }

        public void RevertAllChanges()
        {
            for (int i = MyCategoryTItemSpecs.Count - 1; i >= 0; i--)
            {
                if (MyCategoryTItemSpecs[i].IsDirty)
                {
                    RevertItem(MyCategoryTItemSpecs[i]);
                }
            }
            StateHasChanged();
        }
        #endregion


        #region Button events in the Changes colum   
        public void RestoreItem(TItemSpec item)
        {
            TItemSpec localItem = GetItemFromCollection(MyCategoryTItemSpecs, item);
            if (localItem != null)
            {
                localItem.IsDeleted = false;
            }
        }

        public void RevertItem(TItemSpec item)
        {
            if (item.IsNew)
            {
                MyCategoryTItemSpecs.Remove(item);
            }
            if (item.IsDeleted)
            {
                item.IsDeleted = false;
                ChangeLocalItem(item);
            }
            if (item.IsChanged)
            {
                TItemSpec pristineItem = GetItemFromCollection(PristineItems, item);
                if (pristineItem != null)
                {
                    ChangeLocalItem(pristineItem);
                    PristineItems.Remove(pristineItem);
                    pristineItem.DirtyFields = new List<string>();
                }
            }
        }

        public void DeleteItem(TItemSpec itmToDelete)
        {
            TItemSpec localItem = GetItemFromCollection(MyCategoryTItemSpecs, itmToDelete);
            if (localItem != null)
            {
                if (localItem.IsDeleted)
                {
                    return;
                }
                else if (localItem.IsNew)
                {
                    MyCategoryTItemSpecs.Remove(localItem);
                }
                else
                {
                    localItem.IsDeleted = true;
                }
            }
        }
        #endregion

        #region Helpers
        private void ChangeLocalItem(TItemSpec item)
        {
            var index = MyCategoryTItemSpecs.ToList().FindIndex(i => i.TestNo == item.TestNo);

            if (index != -1)
            {
                var existingItem = MyCategoryTItemSpecs[index];

                if (SelectedItems.Contains(existingItem))
                {
                    var tempSelectedItems = SelectedItems.ToList();

                    tempSelectedItems.Remove(existingItem);
                    tempSelectedItems.Add(item);

                    MyCategoryTItemSpecs[index] = item;

                    SelectedItems = new List<TItemSpec>(tempSelectedItems);
                }
                else
                {
                    MyCategoryTItemSpecs[index] = item;
                }
            }
        }

        private TItemSpec GetItemFromCollection(IList<TItemSpec> collection, TItemSpec itmToFind)
        {
            var index = collection.ToList().FindIndex(i => i.TestNo == itmToFind.TestNo);
            if (index != -1)
            {
                return collection[index];
            }
            return null;
        }
        #endregion


        #region Batch Saving
        public async Task SaveAllChanges()
        {
            List<TItemSpec> deletedItems = MyCategoryTItemSpecs.Where(itm => itm.IsDeleted == true).ToList();
            List<TItemSpec> newItems = MyCategoryTItemSpecs.Where(itm => itm.IsNew == true).ToList();
            List<TItemSpec> updatedItems = MyCategoryTItemSpecs.Where(itm => itm.IsChanged == true && itm.IsDeleted == false).ToList();

            // clean up current data and selection
            MyCategoryTItemSpecs.Clear();
            SelectedItems = Enumerable.Empty<TItemSpec>();

            // update the grid with the data from the service
            List<TItemSpec> newData = await BatchUpdate(deletedItems, newItems, updatedItems);
            MyCategoryTItemSpecs = new ObservableCollection<TItemSpec>(newData);
        }

        private List<TItemSpec> Data { get; set; }
        public Task<List<TItemSpec>> BatchUpdate(
            List<TItemSpec> deletedItems, List<TItemSpec> insertedItems, List<TItemSpec> updatedItems)
        {
            //just sample CRUD operations
            //this is a singleton service to cater for all users at the same time
            //in a real app it may be transient instead
            //also, this code does not cater for concurrency conflicts and errors
            //while a real service should take them into account
            //e.g., insert instead of attempt an update on a missing item that another user deleted
            //in this example this also returns the newly updated data for the grid
            foreach (TItemSpec item in deletedItems)
            {
                Data.Remove(item);
            }

            foreach (TItemSpec item in insertedItems)
            {
                item.TestNo = Data.Max(item => item.TestNo) + 1;
                Data.Insert(0, item);
            }

            foreach (TItemSpec item in updatedItems)
            {
                var index = Data.FindIndex(i => i.TestNo == item.TestNo);
                if (index != -1)
                {
                    Data[index] = item;
                }
            }

            //clean up the view model information to be sure we do not "predefine" user actions
            foreach (TItemSpec item in Data)
            {
                item.IsChanged = false;
                item.IsDeleted = false;
                item.IsNew = false;
                item.DirtyFields = new List<string>();
            }

            return Task.FromResult(Data);
        }
        #endregion
*/
    }
}
