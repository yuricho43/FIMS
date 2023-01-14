using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

//using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

using Fims.Client.Shared.Infrastructure.Extensions;
using Fims.Data.Models;
using Fims.Data.Models.Identity;
using Fims.Client.Shared.Infrastructure;


namespace Fims.Client.Shared.ClientServices.Authentication
{
    public class AuthClientService : IAuthClientService
    {
        private readonly HttpClient httpClient;
        private readonly LocalStorageInterop localStorage; //JBH: changed from ILocalStorageService
        private readonly AuthenticationStateProvider authenticationStateProvider;

        private const string LoginPath = "api/identity/login";
        private const string RegisterPath = "api/identity/register";
        private const string AllUsersPath = "api/identity/getallusers";

        public AuthClientService(
            HttpClient httpClient,
            LocalStorageInterop localStorage, //JBH: changed from ILocalStorageService
            AuthenticationStateProvider authenticationStateProvider)
        {
            this.httpClient = httpClient;
            this.localStorage = localStorage;
            this.authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<Result> Register(RegisterRequestModel model)
            => await this.httpClient
                .PostAsJsonAsync(RegisterPath, model)
                .ToResult();

        public async Task<Result> Login(LoginRequestModel model)
        {
            var response = await this.httpClient.PostAsJsonAsync(LoginPath, model);

            if (!response.IsSuccessStatusCode)
            {
                var errors = await response.Content.ReadFromJsonAsync<string[]>();

                return Result.Failure(errors);
            }

            var responseAsString = await response.Content.ReadAsStringAsync();

            var responseObject = JsonSerializer.Deserialize<LoginResponseModel>(responseAsString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var token = responseObject.Token;

            await this.localStorage.SetItem("authToken", token); //JBH: changed from SetItemAsync

            ((ApiAuthenticationStateProvider)this.authenticationStateProvider).MarkUserAsAuthenticated(model.Email);

            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return Result.Success;
        }

        public async Task Logout()
        {
            await this.localStorage.RemoveItem("authToken"); //JBH: changed from RemoveItemAsync

            ((ApiAuthenticationStateProvider)this.authenticationStateProvider).MarkUserAsLoggedOut();

            this.httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<List<UserAuthInfoModel>> AllUsers()
        {
            var response = await this.httpClient.GetFromJsonAsync<List<UserAuthInfoModel>>(AllUsersPath);
            return response;
        }
    }
}
