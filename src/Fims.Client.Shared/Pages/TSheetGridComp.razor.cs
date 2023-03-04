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
    public partial class TSheetGridComp
    {
        [Parameter]
        public TSheetSpec MyTSheetSpec { get; set; }
        public ObservableCollection<TItemSpec> MyObservableTItemSpecs { get; set; }
        public int MaxChannels { get; set; }

        private List<TItemSpec> PristineItems { get; set; } = new List<TItemSpec>();

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

        public void OnCellRenderHandler(GridCellRenderEventArgs args, string field)
        {
            var itemspec = args.Item as TItemSpec;
            args.Class = !itemspec.IsNew && itemspec.DirtyFields.Contains(field) == true ? "k-changed-cell" : string.Empty;

            if (field == "Ch1Data")
            {
                if (!itemspec.IsCh1DataEnabled)
                    args.Class = "k-disabled-cell";
            }
            else if (field == "Ch2Data")
            {
                if (!itemspec.IsCh2DataEnabled)
                    args.Class = "k-disabled-cell";
            }
            else if (field == "Ch3Data")
            {
                if (!itemspec.IsCh3DataEnabled)
                    args.Class = "k-disabled-cell";
            }
            else if (field == "Ch4Data")
            {
                if (!itemspec.IsCh4DataEnabled)
                    args.Class = "k-disabled-cell";
            }

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

        public void OnRowRenderHandler(GridRowRenderEventArgs args)
        {
            var itemspec = args.Item as TItemSpec;
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

        private void ChangeLocalItem(TItemSpec item)
        {
            var index = MyObservableTItemSpecs.ToList().FindIndex(i => i.TestNo == item.TestNo);
            if (index != -1)
            {
                MyObservableTItemSpecs[index] = item;
            }
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
