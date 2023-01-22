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
        public bool AddUserDialogVisible { get; set; } = false;

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
        }

        private void CreateItem(GridCommandEventArgs args)
        {
            //ProductService.CreateProduct((ProductDto)args.Item);
            //LoadData();
        }

        private void DeleteItem(GridCommandEventArgs args)
        {
            //ProductService.DeleteProduct((ProductDto)args.Item);
            //LoadData();
        }

        private void UpdateItem(GridCommandEventArgs args)
        {
            //ProductService.UpdateProduct((ProductDto)args.Item);
            //LoadData();
        }

        private void ShowAddUserDialog(GridCommandEventArgs args)
        {
            //ProductService.UpdateProduct((ProductDto)args.Item);
            //LoadData();
            AddUserDialogVisible = true;
            //StateHasChanged();
        }

        private async void OnAddUserFinished(RegisterRequestModel newRegisterRequestModel)
        {
            AddUserDialogVisible = false;
            await LoadData();
        }


    }
}
