using Fims.Data.Entities;
using Fims.Data.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Blazor.Components;

namespace Fims.Client.Shared.Pages.Management
{
    public partial class UserManagement
    {
        int PageSize = 15;
        private readonly RegisterRequestModel model = new RegisterRequestModel();
        public List<UserAuthInfoModel> GridData { get; set; }
        public List<FimsRole> UserRoles { get; set; }
        public List<string> UserRoleNames { get; set; } = new List<string>();
        public bool EnabledValidation { get; set; } = true;
        public bool ShowAddUserForm { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            GridData = await this.AuthClientService.AllUsers();

            UserRoles = await this.AuthClientService.AllRoles();
            foreach (var userRole in UserRoles) {
                UserRoleNames.Add(userRole.Name);
            }

            int cool = 7;
        }

        private void CreateItem(GridCommandEventArgs args)
        {
            int cool = 7;
            //ProductService.CreateProduct((ProductDto)args.Item);
            //LoadData();
        }

        private void DeleteItem(GridCommandEventArgs args)
        {
            int cool = 7;
            //ProductService.DeleteProduct((ProductDto)args.Item);
            //LoadData();
        }

        private void UpdateItem(GridCommandEventArgs args)
        {
            int cool = 7;
            //ProductService.UpdateProduct((ProductDto)args.Item);
            //LoadData();
        }

        private void OnAddUserClicked(GridCommandEventArgs args)
        {
            //ProductService.UpdateProduct((ProductDto)args.Item);
            //LoadData();
            ShowAddUserForm = true;
            StateHasChanged();
        }


    }
}
