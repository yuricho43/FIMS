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
            /*
             *  JWT Token consists of 3 parts separated by "."
             *  
             *      - Header (Algorithm & Token type)
             *  
             *      - Payload --> ClaimsPrincipal
             *          . ClaimTypes.NameIdentifier : "f01b2252-3710-4e64-a45a-e285c9eee85f"  (this is the ID index in Db)
             *          . ClaimTypes.Email:   "inspector@fstc.co.kr"
             *          . ClaimTypes.Name:    "김철수"
             *          . ClaimTypes.SurName: "KCS"
             *          . ClaimTypes.Role:    "Inspector"
             *  
             *      - Signature
             */


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName)
            };

            var isAdministrator = await this.userManager.IsInRoleAsync(user, AdministratorRole);
            if (isAdministrator)
            {
                claims.Add(new Claim(ClaimTypes.Role, AdministratorRole));
            }

            var isManager = await this.userManager.IsInRoleAsync(user, ManagerRole);
            if (isManager)
            {
                claims.Add(new Claim(ClaimTypes.Role, ManagerRole));
            }

            var isInspector = await this.userManager.IsInRoleAsync(user, InspectorRole);
            if (isInspector)
            {
                claims.Add(new Claim(ClaimTypes.Role, InspectorRole));
            }

            var secret = Encoding.UTF8.GetBytes(this.applicationSettings.Secret); //JBH: Secret comes from "ApplicationSettings" section @ appsettings.json

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256));

            var tokenHandler = new JwtSecurityTokenHandler();
            var encryptedToken = tokenHandler.WriteToken(token);

            return encryptedToken;
        }
    }
}
