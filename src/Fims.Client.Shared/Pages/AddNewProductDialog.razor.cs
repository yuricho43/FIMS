using AutoMapper;
using Fims.Client.Shared.ClientModels;
using Fims.Client.Shared.ClientServices.TSheetSpecs;
using Fims.Client.Shared.Infrastructure.Extensions;
using Fims.Common;
using Fims.Common.Mapping;
using Fims.Data.Entities;
using Fims.Data.Models;
using Fims.Data.Models.TSheetSpecs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
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
    public partial class AddNewProductDialog
    {
        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        [Parameter]
        public EventCallback<TProductSpec> ProductAdded { get; set; }

        [Parameter]
        public List<string> ProductModels { get; set; }

        public string ProductSerial { get; set; }
        public string ProductModel { get; set; }
        public string Customer { get; set; }
        public string EndUser { get; set; }
        protected string ProductType { get; set; } = "신규";

        protected List<string> ProductTypes = new List<string>() { "신규", "수리" };

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
            //var authState = await AuthenticationStateTask;
            //var user = authState.User;
            var name = user.GetHangulName();

            //ProductModels = await TSheetSpecsClientService.GetEquipmentModelsAsync();
        }

        private void OnBarcodeSelectionClicked()
        {
            BarcodeSelectionDialogVisible = true;
        }

        private void OnProgessListClicked()
        {
            ProgressListDialogVisible = true;
        }


        private void OnAddNewProductDialogOK()
        {
            var product = new TProductSpec
            {
                ProductSerial = ProductSerial,
                ProductModel  = ProductModel,
                Customer      = Customer,
                EndUser       = EndUser,
                ProductType   = ProductType,
            };

            ProductAdded.InvokeAsync(product); // pass Param to parent, by calling EventCallback
        }

        private void OnAddNewProductDialogCancel()
        {
            var product = new TProductSpec
            {
                ProductSerial = "SSSSSSSS",
                ProductModel  = "MMMMMMMM", // mark Invalid
                Customer      = Customer,
                EndUser       = EndUser,
                ProductType   = ProductType,
            };

            ProductAdded.InvokeAsync(product);
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
