using AutoMapper;
using Fims.Client.Shared.ClientServices.TSheets;
using Fims.Common;
using Fims.Data.Entities;
using Fims.Data.Models;
using Fims.Data.Models.TSheets;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using Telerik.Blazor;
using Telerik.Blazor.Components;
using Telerik.DataSource;
using static System.Net.Mime.MediaTypeNames;

namespace Fims.Client.Shared.Pages.Management
{
    public partial class TSheetDetails
    {
        TelerikGrid<TItem> TItemGrid { get; set; }

        [Parameter]
        public string TSheetIdString{ get; set; }

        public TSheet TSheetWithTItems { get; set; }
        //public List<TItem> TItems { get; set; } = new List<TItem>();
        public int MaxChannels { get; set; }

        public ObservableCollection<TItem> MyObservableTItems { get; set; }

        private List<TItem> PristineItems { get; set; } = new List<TItem>();
        public IEnumerable<TItem> SelectedItems { get; set; } = Enumerable.Empty<TItem>();

        // public bool GridIsDirty => MyObservableTItems.ToList().Exists(itm => itm.IsDirty);
        // public bool SelectionIsDirty => SelectedItems.ToList().Exists(itm => itm.IsDirty);

        int PageSize = 10;
        int CurrentPage = 1;

        public string TextBoxFillMode { get; set; } = ThemeConstants.TextBox.FillMode.Solid;
        public string TextBoxRounded { get; set; } = ThemeConstants.TextBox.Rounded.Medium;
        public string TextBoxSize { get; set; } = ThemeConstants.TextBox.Size.Medium;


        //protected override void OnInitialized()
        //{
        //    // MyObservableTItems = new ObservableCollection<TItem>(MyTSheetSpec.TItems);
        //    // Layout.DocsTitle = Localizer["HumanCapital"];
        //    // base.OnInitialized();
        //}

        protected override async Task OnInitializedAsync()
        {
            TSheetWithTItems = await TSheetsClientService.FindTSheetWithDetailsByIdAsync(Int32.Parse(TSheetIdString));
            MaxChannels = TSheetWithTItems.TItems.Select(x => x.Channels).Max();
            await base.OnInitializedAsync();
        }

        public void OnCellRenderHandler(GridCellRenderEventArgs args, string field)
        {
            var itemspec = args.Item as TItem;
            // args.Class = !itemspec.IsNew && itemspec.DirtyFields.Contains(field) == true ? "k-changed-cell" : string.Empty;
            // 
            // if (field == "Ch1Data")
            // {
            //     if (!itemspec.IsCh1DataEnabled)
            //         args.Class = "k-disabled-cell";
            // }
            // else if (field == "Ch2Data")
            // {
            //     if (!itemspec.IsCh2DataEnabled)
            //         args.Class = "k-disabled-cell";
            // }
            // else if (field == "Ch3Data")
            // {
            //     if (!itemspec.IsCh3DataEnabled)
            //         args.Class = "k-disabled-cell";
            // }
            // else if (field == "Ch4Data")
            // {
            //     if (!itemspec.IsCh4DataEnabled)
            //         args.Class = "k-disabled-cell";
            // }
        }

        public void OnRowRenderHandler(GridRowRenderEventArgs args)
        {
            var itemspec = args.Item as TItem;
            ////args.Class = itemspec.IsCh1DataValid ? "" : "userinput-invalid";
            //if (itemspec.IsCh1DataValid)
            //{
            //    args.Class = "";
            //}
            //else
            //{
            //    args.Class = "userinput-invalid";
            //}
        }

        private void OnBackToTSheetManagement()
        {
            this.NavigationManager.NavigateTo("/Management/TSheetManagement", forceLoad: false);
        }


        [CascadingParameter]
        public DialogFactory Dialogs { get; set; }
        public async Task ActivateAlert(string title, string message)
        {
            if (string.IsNullOrWhiteSpace(title))   title = "Warning!";
            if (string.IsNullOrWhiteSpace(message)) message = "Something went wrong!";

            await Dialogs.AlertAsync(message, title);
        }

        #region TextBoxOnChangeHandlers
        //private void TextBoxOnChangeHandler1(object theUserInput, TItem itemspec)
        //{
        //    if (itemspec.ExpressionMode == "Number")
        //    {
        //        bool needToCheckLcl = true;
        //        bool needToCheckUcl = true;

        //        if (Decimal.TryParse(theUserInput as string, out decimal inputval))
        //        {
        //            if (!Decimal.TryParse(itemspec.Ch1LCL, out decimal lcl))
        //                needToCheckLcl = false; //LCL is not defined properly, so skip the check.

        //            if (!Decimal.TryParse(itemspec.Ch1UCL, out decimal ucl))
        //                needToCheckUcl = false; //UCL is not defined properly, so skip the check.

        //            if ((needToCheckLcl) && (inputval < lcl))
        //            {
        //                MarkCh1DataInvalid(itemspec);
        //            }
        //            else
        //            {
        //                MarkCh1DataValid(itemspec);
        //            }

        //            if ((needToCheckUcl) && (inputval > ucl))
        //            {
        //                MarkCh1DataInvalid(itemspec);
        //            }
        //            else
        //            {
        //                MarkCh1DataValid(itemspec);
        //            }
        //        }
        //        else
        //        { 
        //            //UserInput is not a number string
        //            MarkCh1DataInvalid(itemspec);
        //        }
        //    }
        //    else
        //    {
        //        //String, String/Combo
        //        if (string.IsNullOrEmpty(theUserInput as string))
        //        {
        //            MarkCh1DataInvalid(itemspec);
        //        }
        //        else
        //        {
        //            MarkCh1DataValid(itemspec);
        //        }
        //    }
        //}

        //private void MarkCh1DataValid(TItem itemspec)
        //{
        //    itemspec.IsCh1DataValid = true;
        //}

        //private void MarkCh1DataInvalid(TItem itemspec)
        //{
        //    itemspec.Ch1Data = "대한민국";
        //    itemspec.IsCh1DataValid = false;

        //    TextBoxFillMode = ThemeConstants.TextBox.FillMode.Outline;
        //    TextBoxRounded = ThemeConstants.TextBox.Rounded.Full;
        //    //update the UI
        //    //JBH FIXME: Really Needed?    StateHasChanged();
        //}

        //private void TextBoxOnChangeHandler2(object theUserInput, TItem itemspec)
        //{
        //    // the handler receives an object that you may need to cast
        //    string result = string.Format("The user entered: {0}", theUserInput);

        //    //var myTItem = MyTSheetSpec.TItems.Single<TItem>(x => x.TestNo == itemspec.TestNo);
        //    var cool = itemspec.Ch2LCL;
        //    itemspec.Ch2LCL = "위대한";
        //    //var one = itemspec.Ch2LCL;
        //    //var two = myTItem.Ch2LCL;

        //    TextBoxFillMode = ThemeConstants.TextBox.FillMode.Outline;
        //    TextBoxRounded = ThemeConstants.TextBox.Rounded.Full;

        //    //update the UI
        //    //JBH FIXME: Really Needed?    StateHasChanged();
        //}

        //private void TextBoxOnChangeHandler3(object theUserInput, TItem itemspec)
        //{
        //    // the handler receives an object that you may need to cast
        //    string result = string.Format("The user entered: {0}", theUserInput);

        //    //var myTItem = MyTSheetSpec.TItems.Single<TItem>(x => x.TestNo == itemspec.TestNo);
        //    var cool = itemspec.Ch3LCL;
        //    itemspec.Ch3LCL = "자유우선";
        //    //var one = itemspec.Ch3LCL;
        //    //var two = myTItem.Ch3LCL;

        //    TextBoxFillMode = ThemeConstants.TextBox.FillMode.Outline;
        //    TextBoxRounded = ThemeConstants.TextBox.Rounded.Full;

        //    //update the UI
        //    //JBH FIXME: Really Needed?    StateHasChanged();
        //}

        //private void TextBoxOnChangeHandler4(object theUserInput, TItem itemspec)
        //{
        //    // the handler receives an object that you may need to cast
        //    string result = string.Format("The user entered: {0}", theUserInput);

        //    //var myTItem = MyTSheetSpec.TItems.Single<TItem>(x => x.TestNo == itemspec.TestNo);
        //    var cool = itemspec.Ch4LCL;
        //    itemspec.Ch4LCL = "민주국가";
        //    //var one = itemspec.Ch4LCL;
        //    //var two = myTItem.Ch4LCL;

        //    TextBoxFillMode = ThemeConstants.TextBox.FillMode.Outline;
        //    TextBoxRounded = ThemeConstants.TextBox.Rounded.Full;

        //    //update the UI
        //    //JBH FIXME: Really Needed?    StateHasChanged();
        //}

        //private void TextBoxOnBlurHandler(object theUserInput)
        //{
        //    // the handler receives an object that you may need to cast
        //    string result = string.Format("The user entered: {0}", theUserInput);
        //}
        //private void TextBoxValueChangedHandler(object theUserInput)
        //{
        //    // the handler receives an object that you may need to cast
        //    string result = string.Format("The user entered: {0}", theUserInput);
        //}
        #endregion


        #region EditorRegion
        //public void EditHandler(GridCommandEventArgs args)
        //{
        //    TItem item = (TItem)args.Item;

        //    //prevent opening for edit for deleted items
        //    if (item.IsDeleted)
        //    {
        //        args.IsCancelled = true;
        //    }

        //    //show notification
        //}

        //public void UpdateHandler(GridCommandEventArgs args)
        //{
        //    TItem item = (TItem)args.Item;

        //    if (!item.IsDirty)
        //    {
        //        TItem pristineItem = GetItemFromCollection(PristineItems, item);
        //        if (pristineItem == null)
        //        {
        //            //add only the first time a field is edited, later it is no longer pristine
        //            PristineItems.Add(GetItemFromCollection(MyObservableTItems, item));
        //        }
        //    }

        //    item.IsChanged = true;
        //    ChangeLocalItem(item);
        //}

        //public void CreateHandler(GridCommandEventArgs args)
        //{
        //    TItem item = (TItem)args.Item;
        //    item.TestNo = MyObservableTItems.Max(model => model.TestNo) + 1;
        //    item.IsNew = true;
        //    MyObservableTItems.Insert(0, item);
        //}

        //public void DeleteHandler(GridCommandEventArgs args)
        //{
        //    TItem item = (TItem)args.Item;

        //    DeleteItem(item);

        //    //show notification for undelete
        //}

        //private void ChangeLocalItem(TItem item)
        //{
        //    var index = MyObservableTItems.ToList().FindIndex(i => i.TestNo == item.TestNo);
        //    if (index != -1)
        //    {
        //        MyObservableTItems[index] = item;
        //    }
        //}

        //public async Task SaveAllChanges()
        //{
        //    List<TItem> deletedItems = MyObservableTItems.Where(itm => itm.IsDeleted == true).ToList();
        //    List<TItem> newItems = MyObservableTItems.Where(itm => itm.IsNew == true).ToList();
        //    List<TItem> updatedItems = MyObservableTItems.Where(itm => itm.IsChanged == true && itm.IsDeleted == false).ToList();

        //    // clean up current data and selection
        //    MyObservableTItems.Clear();
        //    SelectedItems = Enumerable.Empty<TItem>();

        //    // update the grid with the data from the service
        //    List<TItem> newData = await BatchUpdate(deletedItems, newItems, updatedItems);
        //    MyObservableTItems = new ObservableCollection<TItem>(newData);
        //}

        //public void RevertAllChanges()
        //{
        //    for (int i = MyObservableTItems.Count - 1; i >= 0; i--)
        //    {
        //        if (MyObservableTItems[i].IsDirty)
        //        {
        //            RevertItem(MyObservableTItems[i]);
        //        }
        //    }
        //    StateHasChanged();
        //}

        //public void RestoreItem(TItem item)
        //{
        //    TItem localItem = GetItemFromCollection(MyObservableTItems, item);
        //    if (localItem != null)
        //    {
        //        localItem.IsDeleted = false;
        //    }
        //}

        //public void RevertItem(TItem item)
        //{
        //    if (item.IsNew)
        //    {
        //        MyObservableTItems.Remove(item);
        //    }
        //    if (item.IsDeleted)
        //    {
        //        item.IsDeleted = false;
        //        ChangeLocalItem(item);
        //    }
        //    if (item.IsChanged)
        //    {
        //        TItem pristineItem = GetItemFromCollection(PristineItems, item);
        //        if (pristineItem != null)
        //        {
        //            ChangeLocalItem(pristineItem);
        //            PristineItems.Remove(pristineItem);
        //        }
        //    }
        //}

        //public void RevertSelected()
        //{
        //    foreach (TItem item in SelectedItems)
        //    {
        //        RevertItem(item);
        //    }
        //}

        //public void DeleteItem(TItem itmToDelete)
        //{
        //    TItem localItem = GetItemFromCollection(MyObservableTItems, itmToDelete);
        //    if (localItem != null)
        //    {
        //        if (localItem.IsDeleted)
        //        {
        //            return;
        //        }
        //        else if (localItem.IsNew)
        //        {
        //            MyObservableTItems.Remove(localItem);
        //        }
        //        else
        //        {
        //            localItem.IsDeleted = true;
        //        }
        //    }
        //}

        //public void DeleteSelected()
        //{
        //    foreach (TItem item in SelectedItems)
        //    {
        //        DeleteItem(item);
        //    }
        //}

        //private TItem GetItemFromCollection(IList<TItem> collection, TItem itmToFind)
        //{
        //    var index = collection.ToList().FindIndex(i => i.TestNo == itmToFind.TestNo);
        //    if (index != -1)
        //    {
        //        return collection[index];
        //    }
        //    return null;
        //}

        //private List<TItem> Data { get; set; }
        //public Task<List<TItem>> BatchUpdate(
        //    List<TItem> deletedItems, List<TItem> insertedItems, List<TItem> updatedItems)
        //{
        //    //just sample CRUD operations
        //    //this is a singleton service to cater for all users at the same time
        //    //in a real app it may be transient instead
        //    //also, this code does not cater for concurrency conflicts and errors
        //    //while a real service should take them into account
        //    //e.g., insert instead of attempt an update on a missing item that another user deleted
        //    //in this example this also returns the newly updated data for the grid
        //    foreach (TItem item in deletedItems)
        //    {
        //        Data.Remove(item);
        //    }

        //    foreach (TItem item in insertedItems)
        //    {
        //        item.TestNo = Data.Max(item => item.TestNo) + 1;
        //        Data.Insert(0, item);
        //    }

        //    foreach (TItem item in updatedItems)
        //    {
        //        var index = Data.FindIndex(i => i.TestNo == item.TestNo);
        //        if (index != -1)
        //        {
        //            Data[index] = item;
        //        }
        //    }

        //    //clean up the view model information to be sure we do not "predefine" user actions
        //    foreach (TItem item in Data)
        //    {
        //        item.IsChanged = false;
        //        item.IsDeleted = false;
        //        item.IsNew = false;
        //    }

        //    return Task.FromResult(Data);
        //}
        #endregion
    }
}
