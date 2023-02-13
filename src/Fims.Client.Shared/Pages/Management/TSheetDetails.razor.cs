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

    }
}
