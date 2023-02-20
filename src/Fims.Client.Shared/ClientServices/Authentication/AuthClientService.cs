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
using Microsoft.AspNetCore.Identity;
using Fims.Data.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Fims.Data.Models.TSheetSpecs;
using Fims.Client.Shared.Shared.Common;
using Microsoft.EntityFrameworkCore.Update.Internal;

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
        private const string AllRolesPath = "api/identity/getroles";
        private const string DeletePath = "api/identity/deleteuser";
        private const string ChangeRolePath = "api/identity/changerole";
        private const string ResetPasswordPath = "api/identity/resetpassword";

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
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            string Message = null;
            HttpResponseMessage response = null;
            List<string> errors = new List<string>();
            try
            {
                // GET: api/TSheetSpecs/TSheetSpecByModel/{equipmentModel}
                var result = await this.httpClient.PostAsJsonAsync(LoginPath, model, source.Token);
                if (source?.IsCancellationRequested == false)
                {
                    response = result;
                    if (!response.IsSuccessStatusCode)
                    {
                        errors = await response.Content.ReadFromJsonAsync<List<string>>();
                    }
                }
            }
            catch (Exception e)
            {
                Message = (source?.IsCancellationRequested == true) ? "Request to API timed out" : e.Message;
                Console.WriteLine(Message);
                errors.Add("서버 연결 실패!");
                //_logger.LogError(e, "couldn't retrieve forecast");
            }
            finally
            {
                source = null;
                // poke blazor to reset 
                // in case an error has occurred
                //StateHasChanged();
            }

            if (errors.Count > 0)
            {
                return Result.Failure(errors);

            }


            var responseAsString = await response.Content.ReadAsStringAsync();

            var responseObject = JsonSerializer.Deserialize<LoginResponseModel>(responseAsString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var token = responseObject.Token;

            await this.localStorage.SetItem("authToken", token); //JBH: changed from SetItemAsync

            ((ApiAuthenticationStateProvider)this.authenticationStateProvider).MarkUserAsAuthenticated(model.UserName);

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

        public async Task<List<FimsRole>> AllRoles()
        {
            var response = await this.httpClient.GetFromJsonAsync<List<FimsRole>>(AllRolesPath);
            return response;
        }

        public async Task<Result> Delete(string username)
            => await this.httpClient
                .DeleteAsync(DeletePath + "/" + username)
                .ToResult();

        public async Task<Result> ChangeRole(UserAuthInfoModel model)
        {
            var response = await this.httpClient.PutAsJsonAsync(ChangeRolePath, model);

            if (!response.IsSuccessStatusCode)
            {
                var errors = await response.Content.ReadFromJsonAsync<string[]>();

                return Result.Failure(errors);
            }

            return Result.Success;
        }

        public async Task<Result> ResetPassword(UserAuthInfoModel model)
        {
            var response = await this.httpClient.PutAsJsonAsync(ResetPasswordPath, model);

            if (!response.IsSuccessStatusCode)
            {
                var errors = await response.Content.ReadFromJsonAsync<string[]>();

                return Result.Failure(errors);
            }

            return Result.Success;
        }


    }
}
