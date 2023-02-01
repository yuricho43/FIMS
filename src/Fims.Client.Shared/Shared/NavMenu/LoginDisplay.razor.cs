using System.Security.Cryptography;
using System.Text;
using Fims.Client.Shared.Infrastructure.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;


namespace Fims.Client.Shared.Shared.NavMenu
{
    public partial class LoginDisplay
    {
        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        public string UserName { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateTask;
            if (authState.User.Identity.IsAuthenticated)
            {
                UserName = authState.User.GetUserName();
            }
        }

        private void BeginSignOut(MouseEventArgs args)
        {
            //JBH await SignOutManager.SetSignOutState();
            Navigation.NavigateTo("authentication/logout");
        }

        /// Hashes an email with MD5.  Suitable for use with Gravatar profile
        /// image urls
        string HashEmailForGravatar(string email)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create();
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(email));
            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder("https://www.gravatar.com/avatar/");
            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString(); // Return the hexadecimal string.
        }
    }
}