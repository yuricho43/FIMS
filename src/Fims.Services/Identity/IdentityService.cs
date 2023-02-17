using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Identity;

using Fims.Data.Entities;
using Fims.Data.Models;
using Fims.Data.Models.Identity;
using Microsoft.AspNetCore.Http;
using static System.Runtime.InteropServices.JavaScript.JSType;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;

namespace Fims.Services.Identity
{
    public class IdentityService : IIdentityService
    {
        private const string InvalidErrorMessage = "아이디 또는 암호가 틀렸습니다.";
        private const string UserNoneMessage = "등록된 사용자가 아닙니다";

        private readonly UserManager<FimsUser> userManager;
        private RoleManager<FimsRole> roleManager;
        private readonly IJwtGeneratorService jwtGenerator;

        public IdentityService(
            UserManager<FimsUser> userManager,
            RoleManager<FimsRole> roleManager,
            IJwtGeneratorService jwtGenerator)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.jwtGenerator = jwtGenerator;
        }

        public async Task<Result> RegisterAsync(RegisterRequestModel model)
        {
            var user = new FimsUser
            {
                UserName = model.UserName,
                HangulName = model.HangulName,
                EnglishName = model.HangulName, //use Hangul for now
                Email = model.Email,
                SecurityStamp = "RandomSecurityStamp"
            };

            var identityResult = await this.userManager.CreateAsync(user, model.Password);
            if (identityResult.Succeeded)
            {
                await this.userManager.AddToRoleAsync(user, model.Role);
            }

            var errors = identityResult.Errors.Select(e => e.Description);

            return identityResult.Succeeded
                ? Result.Success
                : Result.Failure(errors);
        }

        public async Task<Result<LoginResponseModel>> LoginAsync(LoginRequestModel model)
        {
            //var user = await this.userManager.FindByEmailAsync(model.Email);
            var user = await this.userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                return InvalidErrorMessage;
            }

            var passwordValid = await this.userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
            {
                return InvalidErrorMessage;
            }

            //debug
            string userId = user.Id;
            var userRoles = await userManager.GetRolesAsync(user);
            var userRole = userRoles.FirstOrDefault();
            //debug

            var token = await this.jwtGenerator.GenerateJwtAsync(user);

            return new LoginResponseModel { Token = token };
        }

        public async Task<Result> ChangeUserProfileAsync(UserProfileModel model, string userId)
        {
            var user = await this.userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return InvalidErrorMessage;
            }

            user.HangulName = model.HangulName;
            user.EnglishName = model.EnglishName;

            var identityResult = await this.userManager.UpdateAsync(user);

            var errors = identityResult.Errors.Select(e => e.Description);

            return identityResult.Succeeded
                ? Result.Success
                : Result.Failure(errors);
        }

        public async Task<Result> ChangePasswordAsync(PasswordModel model, string userId)
        {
            var user = await this.userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return InvalidErrorMessage;
            }

            var identityResult = await this.userManager.ChangePasswordAsync(
                user,
                model.Password,
                model.NewPassword);

            var errors = identityResult.Errors.Select(e => e.Description);

            return identityResult.Succeeded
                ? Result.Success
                : Result.Failure(errors);
        }

        public async Task<Result> ResetPasswordAsync(UserAuthInfoModel model)
        {
            var user = await this.userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                return InvalidErrorMessage;
            }

            var resetPasswordToken = await this.userManager.GeneratePasswordResetTokenAsync(user);

            var identityResult = await this.userManager.ResetPasswordAsync(user, resetPasswordToken, model.Password);
            var errors = identityResult.Errors.Select(e => e.Description);

            return identityResult.Succeeded
               ? Result.Success
               : Result.Failure(errors);
        }

        public async Task<List<UserAuthInfoModel>> AllUsers()
        {
            var users = this.userManager.Users.ToList();
            //var data = await this.TSheetSpecsService.GetEquipmentModelsAsync();

            List<UserAuthInfoModel> userAuthInfos = new List<UserAuthInfoModel>();

            foreach (var user in users)
            {
                var userRoles = await userManager.GetRolesAsync(user);
                var userRole = userRoles.FirstOrDefault();

                var userAuthInfo = new UserAuthInfoModel
                {
                    UserName = user.UserName,
                    Password = "************",
                    Role = userRole,
                    HangulName = user.HangulName,
                    EnglishName = user.EnglishName,
                    Email = user.Email,
                };
                userAuthInfos.Add(userAuthInfo);
            }

            return userAuthInfos;
        }


        public List<FimsRole> Roles()
        {
            var roles = roleManager.Roles;
            var roleslist = roles.ToList();
            return roleslist;
        }

        public async Task<Result> DeleteAsync(string username)
        {
            var user = await this.userManager.FindByNameAsync(username);
            if (user == null)
            {
                return UserNoneMessage;
            }

            var identityResult = await this.userManager.DeleteAsync(user);

            var errors = identityResult.Errors.Select(e => e.Description);

            return identityResult.Succeeded
                ? Result.Success
                : Result.Failure(errors);

        }
    }
}
