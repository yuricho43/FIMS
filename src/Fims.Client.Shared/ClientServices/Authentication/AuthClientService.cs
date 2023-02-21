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
using System.Collections.Generic;
using Telerik.SvgIcons;

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
        private const string ChangeProfilePath = "api/identity/changeprofile";
        private const string ChangePasswordPath = "api/identity/changepassword";
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
        {
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            string Message = null;
            HttpResponseMessage response = null;
            List<string> errors = new List<string>();

            try
            {
                var result = await this.httpClient.PostAsJsonAsync(RegisterPath, model, source.Token);
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
            else
            {
                return Result.Success;
            }
        }

        public async Task<Result> Login(LoginRequestModel model)
        {
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            string Message = null;
            HttpResponseMessage response = null;
            List<string> errors = new List<string>();

            try
            {
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
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            string Message = null;
            List< UserAuthInfoModel>  users = new List<UserAuthInfoModel>();

            try
            {
                var result = await this.httpClient.GetFromJsonAsync<List<UserAuthInfoModel>>(AllUsersPath, source.Token);
                if (source?.IsCancellationRequested == false)
                {
                    users = result;
                }
            }
            catch (Exception e)
            {
                Message = (source?.IsCancellationRequested == true) ? "Request to API timed out" : e.Message;
                Console.WriteLine(Message);
                //_logger.LogError(e, "couldn't retrieve forecast");
            }
            finally
            {
                source = null;
                // poke blazor to reset 
                // in case an error has occurred
                //StateHasChanged();
            }

            return users;
        }

        public async Task<List<FimsRole>> AllRoles()
        {
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            string Message = null;
            List<FimsRole> roles = new List<FimsRole>();

            try
            {
                var result = await this.httpClient.GetFromJsonAsync<List<FimsRole>>(AllRolesPath, source.Token);
                if (source?.IsCancellationRequested == false)
                {
                    roles = result;
                }
            }
            catch (Exception e)
            {
                Message = (source?.IsCancellationRequested == true) ? "Request to API timed out" : e.Message;
                Console.WriteLine(Message);
                //_logger.LogError(e, "couldn't retrieve forecast");
            }
            finally
            {
                source = null;
                // poke blazor to reset 
                // in case an error has occurred
                //StateHasChanged();
            }

            return roles;
        }

        public async Task<Result> Delete(string username)
        {
            //return await this.httpClient.DeleteAsync(DeletePath + "/" + username).ToResult();

            var source = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            string Message = null;
            HttpResponseMessage response = null;
            List<string> errors = new List<string>();

            try
            {
                var result = await this.httpClient.DeleteAsync(DeletePath + "/" + username, source.Token);
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
            else
            {
                return Result.Success;
            }
        }

        public async Task<Result> ChangeRole(UserAuthInfoModel model)
        {
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            string Message = null;
            HttpResponseMessage response = null;
            List<string> errors = new List<string>();

            try
            {
                var result = await this.httpClient.PutAsJsonAsync(ChangeRolePath, model, source.Token);
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
            else
            {
                return Result.Success;
            }
        }


        public async Task<Result> ChangeProfile(UserProfileModel model)
        {
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            string Message = null;
            HttpResponseMessage response = null;
            List<string> errors = new List<string>();

            try
            {           
                var result = await this.httpClient.PutAsJsonAsync(ChangeProfilePath, model, source.Token);
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
            else
            {
                return Result.Success;
            }
        }


        public async Task<Result> ChangePassword(PasswordModel model)
        {
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            string Message = null;
            HttpResponseMessage response = null;
            List<string> errors = new List<string>();

            try
            {
                var result = await this.httpClient.PutAsJsonAsync(ChangePasswordPath, model, source.Token);
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
            else
            {
                return Result.Success;
            }
        }

        public async Task<Result> ResetPassword(UserAuthInfoModel model)
        {
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            string Message = null;
            HttpResponseMessage response = null;
            List<string> errors = new List<string>();

            try
            {
                var result = await this.httpClient.PutAsJsonAsync(ResetPasswordPath, model, source.Token);
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
            else
            {
                return Result.Success;
            }
        }
    }
}
