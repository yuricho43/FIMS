using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Fims.Data.Entities;
using Fims.Data.Models;

using static Fims.Common.Constants;
using Fims.Common;

namespace Fims.Services.Identity
{
    public class JwtGeneratorService : IJwtGeneratorService
    {
        private readonly UserManager<FimsUser> userManager;
        private readonly ApplicationSettings applicationSettings;
        public bool AllowMultiRoles { get; set; } = false;

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
             *          . ClaimTypes.NameIdentifier:        "f01b2252-3710-4e64-a45a-e285c9eee85f"  (this is the ID index in Db)
             *          . CustomClaimTypes.UserName:        "inspector1"
             *          . ClaimTypes.Email:                 "inspector1@fstc.co.kr"
             *          . CustomClaimTypes.HangulName:      "김철수"
             *          . CustomClaimTypes.EnglishName:     "KCS"
             *          . ClaimTypes.Role:                  "Inspector"
             *          
             *      - Signature
             */

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(CustomClaimTypes.UserName, user.UserName),        //NOTE: using CustomClaimTypes
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(CustomClaimTypes.HangulName, user.HangulName),    //NOTE: using CustomClaimTypes
                new Claim(CustomClaimTypes.EnglishName, user.EnglishName)   //NOTE: using CustomClaimTypes
            };

            //JBH: A User can have multiple Roles, for an example,
            //     User "coolbix" may have two Roles such as "InspectorRole" and "ManagerRole".
            //
            //     For Now, let's use the "Single" Role --> AllowMultiRoles==false
            if (AllowMultiRoles)
            {
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

                var isReporter = await this.userManager.IsInRoleAsync(user, ReporterRole);
                if (isReporter)
                {
                    claims.Add(new Claim(ClaimTypes.Role, ReporterRole));
                }
            }
            else
            {
                var userRoles = await this.userManager.GetRolesAsync(user);
                var userRole = userRoles.FirstOrDefault();
                claims.Add(new Claim(ClaimTypes.Role, userRole));
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
