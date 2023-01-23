using System.Collections.Generic;
using System.Threading.Tasks;

using Fims.Data.Models.Identity;


namespace Fims.Client.Shared.Pages.Account
{
    public partial class Login
    {
        private readonly LoginRequestModel model = new LoginRequestModel();

        public bool ShowErrors { get; set; }

        public IEnumerable<string> Errors { get; set; }

        private async Task SubmitAsync()
        {
            var result = await this.AuthClientService.Login(this.model);

            if (result.Succeeded)
            {
                this.ShowErrors = false;
                //this.ToastService.ShowSuccess("You have successfully logged in");
                this.NavigationManager.NavigateTo("/");
                StateHasChanged();
            }
            else
            {
                this.Errors = result.Errors;
                this.ShowErrors = true;
            }
        }
    }
}
