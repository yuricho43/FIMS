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
        public List<UserAuthInfoModel> GridData { get; set; }
        public List<FimsRole> UserRoles { get; set; }
        public List<string> UserRoleNames { get; set; } = new List<string>();
        public bool EnabledValidation { get; set; } = true;
        public bool AddUserDialogVisible { get; set; } = false;
        public TelerikNotification UserManagementNotificationComponent { get; set; }
        public bool ShowErrors { get; set; }
        public IEnumerable<string> Errors { get; set; }

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

        private async Task DeleteUser(GridCommandEventArgs args)
        {
            var userinfo = (UserAuthInfoModel)args.Item;

            var result = await this.AuthClientService.Delete(userinfo.UserName);

            if (result.Succeeded)
            {
                this.ShowErrors = false;

                UserManagementNotificationComponent.Show(new NotificationModel
                {
                    Text = "사용자 계정삭제 성공",
                    ThemeColor = "success",
                    //CloseAfter = 3000
                });
            }
            else
            {
                this.Errors = result.Errors;
                this.ShowErrors = true;
            }

            await LoadData();
        }

        private void UpdateUser(GridCommandEventArgs args)
        {
            //ProductService.UpdateProduct((ProductDto)args.Item);
            //await LoadData();
        }

        private void ShowAddUserDialog(GridCommandEventArgs args)
        {
            //ProductService.UpdateProduct((ProductDto)args.Item);
            //await LoadData();
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
