using AutoMapper;
using Fims.Client.Shared.ClientServices.TSheetSpecs;
using Fims.Common;
using Fims.Data.Models;
using Fims.Data.Models.Identity;
using Fims.Data.Models.TSheetSpecs;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using Telerik.Blazor;
using Telerik.Blazor.Components;
using Telerik.DataSource;
using static System.Net.Mime.MediaTypeNames;

namespace Fims.Client.Shared.Pages
{
    public partial class TSheetCategoryComponent
    {
        [Parameter]
        public ObservableCollection<TItemSpec> TItemSpecsInCategory { get; set; }

        [Parameter]
        public EventCallback<string> TItemSpecsInCategoryCompletedCountChanged { get; set; }

        public int MaxChannels { get; set; }

        private List<TItemSpec> TItemSpecsPristine { get; set; } = new List<TItemSpec>();
        public IEnumerable<TItemSpec> TItemSpecsSelected { get; set; } = Enumerable.Empty<TItemSpec>();

        public bool GridIsDirty => TItemSpecsInCategory.ToList().Exists(item => item.IsDirty);
        public bool SelectionIsDirty => TItemSpecsSelected.ToList().Exists(item => item.IsDirty);

        public int TItemSpecsInCategoryCompletedCount { get; set; } = 0;

        TelerikGrid<TItemSpec> TItemSpecGrid { get; set; }

        int PageSize = 10;
        int CurrentPage = 1;


        public string TextBoxFillMode { get; set; } = ThemeConstants.TextBox.FillMode.Solid;
        public string TextBoxRounded { get; set; } = ThemeConstants.TextBox.Rounded.Medium;
        public string TextBoxSize { get; set; } = ThemeConstants.TextBox.Size.Medium;


        protected override void OnInitialized()
        {
            MaxChannels = TItemSpecsInCategory.Select(x => x.Channels).Max();

            TItemSpecsInCategoryCompletedCount = TItemSpecsInCategory.Where(x => x.Completed==true).Count();
            CalculateTItemSpecsInputCompletedCount();

            Layout.DocsTitle = Localizer["HumanCapital"];
            base.OnInitialized();
        }

        //protected override async Task OnInitializedAsync()
        //{
        //    int cool = 7;
        //    MyTItemSpecs = new ObservableCollection<TItemSpec>(MyTSheetSpec.TItemSpecs);
        //}

        FilterDescriptor SingleTeamDescriptor() => new FilterDescriptor("TeamId", FilterOperator.IsEqualTo, 3);


        [CascadingParameter]
        public DialogFactory Dialogs { get; set; }
        public async Task ActivateAlert(string title, string message)
        {
            if (string.IsNullOrWhiteSpace(title)) title = "Warning!";
            if (string.IsNullOrWhiteSpace(message)) message = "Something went wrong!";

            await Dialogs.AlertAsync(message, title);
        }

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
                TItemSpec pristineItem = GetItemFromCollection(TItemSpecsPristine, itemspec);
                if (pristineItem == null)
                {
                    //add only the first time a field is edited, later it is no longer pristine
                    TItemSpecsPristine.Add(GetItemFromCollection(TItemSpecsInCategory, itemspec));
                }
            }

            itemspec.IsChanged = true;
            itemspec.DirtyFields.Add(args.Field);

            itemspec.Completed = CheckAllChannelDataEntered(itemspec);

            ChangeLocalItem(itemspec);

            CalculateTItemSpecsInputCompletedCount();
        }

        public void CreateHandler(GridCommandEventArgs args)
        {
            TItemSpec item = (TItemSpec)args.Item;
            item.TestNo = TItemSpecsInCategory.Max(model => model.TestNo) + 1;
            item.IsNew = true;
            TItemSpecsInCategory.Insert(0, item);
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
            foreach (TItemSpec item in TItemSpecsSelected)
            {
                DeleteItem(item);
            }

            TItemSpecsSelected = new List<TItemSpec>();
        }

        public void RevertSelected()
        {
            foreach (TItemSpec item in TItemSpecsSelected)
            {
                RevertItem(item);
            }

            TItemSpecsSelected = new List<TItemSpec>();
        }

        public void RevertAllChanges()
        {
            for (int i = TItemSpecsInCategory.Count - 1; i >= 0; i--)
            {
                if (TItemSpecsInCategory[i].IsDirty)
                {
                    RevertItem(TItemSpecsInCategory[i]);
                }
            }
            StateHasChanged();
        }
        #endregion


        #region Button events in the Changes colum   
        public void RestoreItem(TItemSpec item)
        {
            TItemSpec localItem = GetItemFromCollection(TItemSpecsInCategory, item);
            if (localItem != null)
            {
                localItem.IsDeleted = false;
            }
        }

        public void RevertItem(TItemSpec item)
        {
            if (item.IsNew)
            {
                TItemSpecsInCategory.Remove(item);
            }
            if (item.IsDeleted)
            {
                item.IsDeleted = false;
                ChangeLocalItem(item);
            }
            if (item.IsChanged)
            {
                TItemSpec pristineItem = GetItemFromCollection(TItemSpecsPristine, item);
                if (pristineItem != null)
                {
                    ChangeLocalItem(pristineItem);
                    TItemSpecsPristine.Remove(pristineItem);
                    pristineItem.DirtyFields = new List<string>();
                }
            }
        }

        public void DeleteItem(TItemSpec itmToDelete)
        {
            TItemSpec localItem = GetItemFromCollection(TItemSpecsInCategory, itmToDelete);
            if (localItem != null)
            {
                if (localItem.IsDeleted)
                {
                    return;
                }
                else if (localItem.IsNew)
                {
                    TItemSpecsInCategory.Remove(localItem);
                }
                else
                {
                    localItem.IsDeleted = true;
                }
            }
        }
        #endregion

        #region Helpers
        private bool CheckAllChannelDataEntered(TItemSpec itemspec)
        {
            var ch1 = itemspec.IsCh1DataEnabled ? (itemspec.IsCh1DataEntered ? true : false) : true;
            var ch2 = itemspec.IsCh2DataEnabled ? (itemspec.IsCh2DataEntered ? true : false) : true;
            var ch3 = itemspec.IsCh3DataEnabled ? (itemspec.IsCh3DataEntered ? true : false) : true;
            var ch4 = itemspec.IsCh4DataEnabled ? (itemspec.IsCh4DataEntered ? true : false) : true;

            return ch1 && ch2 && ch3 && ch4;
        }

        private void CalculateTItemSpecsInputCompletedCount()
        {
            var compeletedCount = TItemSpecsInCategory.Where(x => x.Completed == true).Count();
            if (compeletedCount != TItemSpecsInCategoryCompletedCount)
            {
                TItemSpecsInCategoryCompletedCount = compeletedCount;
                TItemSpecsInCategoryCompletedCountChanged.InvokeAsync($"{TItemSpecsInCategory[0].Category}:{compeletedCount}"); // notify pass Param to parent, by calling EventCallback
            }
        }

        private void ChangeLocalItem(TItemSpec itemspec)
        {
            var index = TItemSpecsInCategory.ToList().FindIndex(i => i.TestNo == itemspec.TestNo);

            if (index != -1)
            {
                var existingItem = TItemSpecsInCategory[index];

                if (TItemSpecsSelected.Contains(existingItem))
                {
                    var tempSelectedItems = TItemSpecsSelected.ToList();

                    tempSelectedItems.Remove(existingItem);
                    tempSelectedItems.Add(itemspec);

                    TItemSpecsInCategory[index] = itemspec;

                    TItemSpecsSelected = new List<TItemSpec>(tempSelectedItems);
                }
                else
                {
                    TItemSpecsInCategory[index] = itemspec;
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
            List<TItemSpec> deletedItems = TItemSpecsInCategory.Where(item => item.IsDeleted == true).ToList();
            List<TItemSpec> newItems = TItemSpecsInCategory.Where(item => item.IsNew == true).ToList();
            List<TItemSpec> updatedItems = TItemSpecsInCategory.Where(item => item.IsChanged == true && item.IsDeleted == false).ToList();

            // clean up current data and selection
            TItemSpecsInCategory.Clear();
            TItemSpecsSelected = Enumerable.Empty<TItemSpec>();

            // update the grid with the data from the service
            List<TItemSpec> newData = await BatchUpdate(deletedItems, newItems, updatedItems);
            TItemSpecsInCategory = new ObservableCollection<TItemSpec>(newData);
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
    }
}
