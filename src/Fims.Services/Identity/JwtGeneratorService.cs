using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Fims.Data.Entities;
using Fims.Data.Models;

using static Fims.Common.Constants;


namespace Fims.Services.Identity
{
    public class JwtGeneratorService : IJwtGeneratorService
    {
        private readonly UserManager<FimsUser> userManager;
        private readonly ApplicationSettings applicationSettings;

        public JwtGeneratorService(
            UserManager<FimsUser> userManager,
            IOptions<ApplicationSettings> applicationSettings)
        {
            this.userManager = userManager;
            this.applicationSettings = applicationSettings.Value;
        }

        public async Task<string> GenerateJwtAsync(FimsUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName)
            };
            /*
             * claims:	
             *     [0]	{http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier:  c8cd5a1a-077b-448f-817f-4c19b1fa70dc}
             *     [1]	{http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress:    coolbix@hanmail.net}
             *     [2]	{http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name:            Cool}
             *     [3]	{http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname:         Bix}
             */        


            var isAdministrator = await this.userManager.IsInRoleAsync(user, AdministratorRole);

            if (isAdministrator)
            {
                claims.Add(new Claim(ClaimTypes.Role, AdministratorRole));
            }

            var secret = Encoding.UTF8.GetBytes(this.applicationSettings.Secret); //JBH: Secret comes from "ApplicationSettings" section @ appsettings.json

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(secret),
                    SecurityAlgorithms.HmacSha256));

            var tokenHandler = new JwtSecurityTokenHandler();
            var encryptedToken = tokenHandler.WriteToken(token);

            return encryptedToken;
        }
    }
}
