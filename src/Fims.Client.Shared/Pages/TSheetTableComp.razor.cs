using AutoMapper;
using Fims.Client.Shared.ClientServices.TSheetSpecs;
using Fims.Common;
using Fims.Data.Models;
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
    public partial class TSheetTableComp
    {
        [Parameter]
        public TSheetSpec MyTSheetSpec { get; set; }
        public ObservableCollection<TItemSpec> MyObservableTItemSpecs { get; set; }
        public int MaxChannels { get; set; }

        private List<TItemSpec> PristineItems { get; set; } = new List<TItemSpec>();
        public IEnumerable<TItemSpec> SelectedItems { get; set; } = Enumerable.Empty<TItemSpec>();

        public bool GridIsDirty => MyObservableTItemSpecs.ToList().Exists(itm => itm.IsDirty);
        public bool SelectionIsDirty => SelectedItems.ToList().Exists(itm => itm.IsDirty);

        public int ActiveCategoryTabIndex { get; set; } = 1;
        TelerikGrid<TItemSpec> TItemSpecGrid { get; set; }

        public int Page { get; set; } = 1;

        public string TextBoxFillMode { get; set; } = ThemeConstants.TextBox.FillMode.Solid;
        public string TextBoxRounded { get; set; } = ThemeConstants.TextBox.Rounded.Medium;
        public string TextBoxSize { get; set; } = ThemeConstants.TextBox.Size.Medium;


        protected override void OnInitialized()
        {
            MyObservableTItemSpecs = new ObservableCollection<TItemSpec>(MyTSheetSpec.TItemSpecs);
            MaxChannels = MyTSheetSpec.TItemSpecs.Select(x => x.Channels).Max();
            Layout.DocsTitle = Localizer["HumanCapital"];
            base.OnInitialized();
        }

        //protected override async Task OnInitializedAsync()
        //{
        //    int cool = 7;
        //    MyTItemSpecs = new ObservableCollection<TItemSpec>(MyTSheetSpec.TItemSpecs);
        //}

        #region Grid Render Handlers
        public void OnCellRenderHandler(GridCellRenderEventArgs args)
        {
            var itemspec = args.Item as TItemSpec;
            //args.Class = (bool)args.Value ? string.Empty : "userinput-valid";
            if ((bool)args.Value)
            {
                args.Class = string.Empty;
            }
            else
            {
                args.Class = "userinput-valid";
            }

            if (itemspec.IsCh1DataValid)
            {
                args.Class = string.Empty;
            }
            else
            {
                args.Class = "userinput-invalid";
            }

        }

        public void OnRowRenderHandler(GridRowRenderEventArgs args)
        {
            var itemspec = args.Item as TItemSpec;
            //args.Class = itemspec.IsCh1DataValid ? "" : "userinput-invalid";
            if (itemspec.IsCh1DataValid)
            {
                args.Class = "";
            }
            else
            {
                args.Class = "userinput-invalid";
            }
        }
        #endregion


        FilterDescriptor SingleTeamDescriptor() => new FilterDescriptor("TeamId", FilterOperator.IsEqualTo, 3);


        [CascadingParameter]
        public DialogFactory Dialogs { get; set; }
        public async Task ActivateAlert(string title, string message)
        {
            if (string.IsNullOrWhiteSpace(title))   title = "Warning!";
            if (string.IsNullOrWhiteSpace(message)) message = "Something went wrong!";

            await Dialogs.AlertAsync(message, title);
        }

        #region TextBoxOnChangeHandlers
        private void TextBoxOnChangeHandler1(object theUserInput, TItemSpec itemspec, string tboxId)
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
        }

        private void MarkCh1DataInvalid(TItemSpec itemspec)
        {
            itemspec.Ch1Data = "대한민국";
            itemspec.IsCh1DataValid = false;

            TextBoxFillMode = ThemeConstants.TextBox.FillMode.Outline;
            TextBoxRounded = ThemeConstants.TextBox.Rounded.Full;
            //update the UI
            //JBH FIXME: Really Needed?    StateHasChanged();
        }

        private void TextBoxOnChangeHandler2(object theUserInput, TItemSpec itemspec, string tboxId)
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

        private void TextBoxOnChangeHandler3(object theUserInput, TItemSpec itemspec, string tboxId)
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

        private void TextBoxOnChangeHandler4(object theUserInput, TItemSpec itemspec, string tboxId)
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

        /* JBH: GridCommandEventArgs properties
                //     Defines whether the command should be cancelled.
                public bool IsCancelled { get; set; }

                //     Defines the item that has been affected. You can cast it to the model type to
                //     which you bind the grid.
                public object Item { get; set; }

                //     Defines whether the item is recently added by the end user through the interface.
                public bool IsNew { get; set; }

                //     Defines the updated field. Available for incell editing.
                public string Field { get; set; }

                //     Defines the updated value. Available for incell editing.
                public object Value { get; set; }
         */

        #region CRUD Handlers
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
            TItemSpec item = (TItemSpec)args.Item;

            if (!item.IsDirty)
            {
                TItemSpec pristineItem = GetItemFromCollection(PristineItems, item);
                if (pristineItem == null)
                {
                    //add only the first time a field is edited, later it is no longer pristine
                    PristineItems.Add(GetItemFromCollection(MyObservableTItemSpecs, item));
                }
            }

            item.IsChanged = true;
            ChangeLocalItem(item);
        }

        public void CreateHandler(GridCommandEventArgs args)
        {
            TItemSpec item = (TItemSpec)args.Item;
            item.TestNo = MyObservableTItemSpecs.Max(model => model.TestNo) + 1;
            item.IsNew = true;
            MyObservableTItemSpecs.Insert(0, item);
        }

        public void DeleteHandler(GridCommandEventArgs args)
        {
            TItemSpec item = (TItemSpec)args.Item;

            DeleteItem(item);

            //show notification for undelete
        }
        #endregion


        private void ChangeLocalItem(TItemSpec item)
        {
            var index = MyObservableTItemSpecs.ToList().FindIndex(i => i.TestNo == item.TestNo);
            if (index != -1)
            {
                MyObservableTItemSpecs[index] = item;
            }
        }

        public void RevertAllChanges()
        {
            for (int i = MyObservableTItemSpecs.Count - 1; i >= 0; i--)
            {
                if (MyObservableTItemSpecs[i].IsDirty)
                {
                    RevertItem(MyObservableTItemSpecs[i]);
                }
            }
            StateHasChanged();
        }

        public void RestoreItem(TItemSpec item)
        {
            TItemSpec localItem = GetItemFromCollection(MyObservableTItemSpecs, item);
            if (localItem != null)
            {
                localItem.IsDeleted = false;
            }
        }

        public void RevertItem(TItemSpec item)
        {
            if (item.IsNew)
            {
                MyObservableTItemSpecs.Remove(item);
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
                }
            }
        }

        public void RevertSelected()
        {
            foreach (TItemSpec item in SelectedItems)
            {
                RevertItem(item);
            }
        }

        public void DeleteItem(TItemSpec itmToDelete)
        {
            TItemSpec localItem = GetItemFromCollection(MyObservableTItemSpecs, itmToDelete);
            if (localItem != null)
            {
                if (localItem.IsDeleted)
                {
                    return;
                }
                else if (localItem.IsNew)
                {
                    MyObservableTItemSpecs.Remove(localItem);
                }
                else
                {
                    localItem.IsDeleted = true;
                }
            }
        }

        public void DeleteSelected()
        {
            foreach (TItemSpec item in SelectedItems)
            {
                DeleteItem(item);
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
    }
}
