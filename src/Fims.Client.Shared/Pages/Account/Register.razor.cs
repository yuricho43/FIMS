using System.Collections.Generic;
using System.Threading.Tasks;

using Fims.Data.Models.Identity;


namespace Fims.Client.Shared.Pages.Account
{
    public partial class Register
    {
        private readonly RegisterRequestModel model = new RegisterRequestModel();

        public bool ShowErrors { get; set; }

        public IEnumerable<string> Errors { get; set; }

        private async Task SubmitAsync()
        {
            var result = await this.AuthClientService.Register(this.model);

            if (result.Succeeded)
            {
                this.ShowErrors = false;

                // this.ToastService.ShowSuccess(
                //     "You have successfully registered.\n Please login.",
                //     "Congratulations!");
                this.ToastService.ShowSuccess(
                    "You have successfully registered.\n Please login.");

                this.NavigationManager.NavigateTo("/account/login");
            }
            else
            {
                this.Errors = result.Errors;
                this.ShowErrors = true;
            }
        }
    }
}
