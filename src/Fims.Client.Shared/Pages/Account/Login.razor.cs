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
        public bool InProgress { get; set; } = false;

        private async Task SubmitAsync()
        {
            InProgress = true;
            var result = await this.AuthClientService.Login(this.model);

            if (result.Succeeded)
            {
                this.ShowErrors = false;
                //this.ToastService.ShowSuccess("You have successfully logged in");
                StateHasChanged();
                this.NavigationManager.NavigateTo("/");
            }
            else
            {
                this.Errors = result.Errors;
                this.ShowErrors = true;
            }

            InProgress = false;
        }
    }
}
