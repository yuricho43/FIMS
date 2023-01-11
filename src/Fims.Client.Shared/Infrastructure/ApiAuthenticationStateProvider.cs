using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

//using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;


namespace Fims.Client.Shared.Infrastructure
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient httpClient;
        //private readonly ILocalStorageService localStorage;
        private readonly LocalStorageInterop localStorage;

        public ApiAuthenticationStateProvider(
            HttpClient httpClient,
            LocalStorageInterop localStorage) //JBH: changed from ILocalStorageService
        {
            this.httpClient = httpClient;
            this.localStorage = localStorage;
        }

        public void MarkUserAsAuthenticated(string userName)
        {
            var authenticatedUser = new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, userName)
                }, "apiauth"));

            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));

            NotifyAuthenticationStateChanged(authState);
        }

        public void MarkUserAsLoggedOut()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));

            NotifyAuthenticationStateChanged(authState);
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var savedToken = await this.localStorage.GetItem<string>("authToken"); //JBH: changed from GetItemAsync

            if (string.IsNullOrWhiteSpace(savedToken))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(this.ParseClaimsFromJwt(savedToken), "jwt")));
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            /*
             *  JWT Token consists of 3 parts separated by "."
             *  
             *      - Header (Algorithm & Token type)
             *  
             *      - Payload --> ClaimsPrincipal
             *          . ClaimTypes.NameIdentifier : "f01b2252-3710-4e64-a45a-e285c9eee85f"  (this is the ID index in Db)
             *          . ClaimTypes.Email:   "worker@fstc.co.kr"
             *          . ClaimTypes.Name:    "김철수"
             *          . ClaimTypes.SurName: "KCS"
             *          . ClaimTypes.Role:    "Worker"
             *  
             *      - Signature
             */

            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = this.ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            keyValuePairs.TryGetValue(ClaimTypes.Role, out var roles);

            if (roles != null)
            {
                if (roles.ToString().Trim().StartsWith("["))
                {
                    var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

                    claims.AddRange(parsedRoles.Select(role => new Claim(ClaimTypes.Role, role)));
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
                }

                keyValuePairs.Remove(ClaimTypes.Role);
            }

            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));

            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            return Convert.FromBase64String(base64);
        }
    }
}
