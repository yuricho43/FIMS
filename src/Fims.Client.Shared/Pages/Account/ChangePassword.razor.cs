using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;

using Fims.Data.Models.Identity;


namespace Fims.Client.Shared.Pages.Account
{
    public partial class ChangePassword
    {
        private readonly PasswordModel ChangePasswordModel = new PasswordModel();

        public bool ShowErrors { get; set; }

        public IEnumerable<string> Errors { get; set; }

        private async Task SubmitAsync()
        {
            var response = await this.Http.PutAsJsonAsync("api/identity/changepassword", this.ChangePasswordModel);

            if (response.IsSuccessStatusCode)
            {
                this.ShowErrors = false;

                this.ChangePasswordModel.Password = null;
                this.ChangePasswordModel.NewPassword = null;
                this.ChangePasswordModel.ConfirmNewPassword = null;

                await this.AuthClientService.Logout();

                //this.ToastService.ShowSuccess("Your password has been changed successfully.\n Please login.");
                this.NavigationManager.NavigateTo("/account/login");
            }
            else
            {
                this.Errors = await response.Content.ReadFromJsonAsync<string[]>();
                this.ShowErrors = true;
            }
        }
    }
}
