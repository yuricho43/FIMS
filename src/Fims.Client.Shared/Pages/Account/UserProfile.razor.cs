using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;

using Fims.Client.Shared.Infrastructure.Extensions;
using Fims.Data.Models.Identity;

namespace Fims.Client.Shared.Pages.Account
{
    public partial class UserProfile
    {
        private readonly ChangeUserProfileRequestModel model = new ChangeUserProfileRequestModel();

        private string email;

        public bool ShowErrors { get; set; }

        public IEnumerable<string> Errors { get; set; }

        protected override async Task OnInitializedAsync() => await this.LoadDataAsync();

        private async Task SubmitAsync()
        {
            var response = await this.Http.PutAsJsonAsync("api/identity/changeuserprofile", this.model);

            if (response.IsSuccessStatusCode)
            {
                this.ShowErrors = false;

                await this.AuthClientService.Logout();

                //this.ToastService.ShowSuccess("Your account UserProfile has been changed successfully.\n Please login.");
                this.NavigationManager.NavigateTo("/account/login");
            }
            else
            {
                this.Errors = await response.Content.ReadFromJsonAsync<string[]>();
                this.ShowErrors = true;
            }
        }

        private async Task LoadDataAsync()
        {
            var state = await this.AuthState.GetAuthenticationStateAsync();
            var user = state.User;
            
            this.email = user.GetEmail();
            this.model.HangulName  = user.GetHangulName();
            this.model.EnglishName = user.GetEnglishName();
        }
    }
}
