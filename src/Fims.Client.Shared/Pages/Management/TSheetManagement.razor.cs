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
using Fims.Data.Models.Identity;

namespace Fims.Client.Shared.Pages.Management
{
    public partial class TSheetManagement
    {
        TelerikGrid<TSheet> TSheetManagementGridRef { get; set; }

        public IEnumerable<TSheet> TSheets { get; set; } = Enumerable.Empty<TSheet>();
        public IEnumerable<TSheet> SelectedTSheets { get; set; } = Enumerable.Empty<TSheet>();
        public TSheet CurrentTSheet { get; set; }

        public bool GenerateTReportDialogVisible { get; set; } = false;
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
            TReportSpecs = await TReportsClientService.AllTReportSpecs();
        }

        private void OnGenerateTReportFinished(string fileGenerated)
        {
            GenerateTReportDialogVisible = false;
            //await LoadData();
            //StateHasChanged();
        }

        public void ShowDetailsHandler(GridCommandEventArgs args)
        {
            var tSheet = (TSheet)args.Item;
            this.NavigationManager.NavigateTo($"/Management/TSheetDetails/{tSheet.Id}", forceLoad: true);
        }

        public void GenerateReportHandler(GridCommandEventArgs args)
        {
            CurrentTSheet = (TSheet)args.Item;
            GenerateTReportDialogVisible = true;
            //StateHasChanged();
        }

        public async void DeleteTSheetHandler(GridCommandEventArgs args)
        {
            // var tSheet = (TSheet)args.Item;
            // var result = await this.TSheetsClientService.RemoveTSheet(tSheet.Id);
            // 
            // TSheets = await TSheetsClientService.AllTSheetsAsync();
            // //StateHasChanged();
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

    }
}
