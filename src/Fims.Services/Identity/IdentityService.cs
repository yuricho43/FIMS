using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Identity;

using Fims.Data.Entities;
using Fims.Data.Models;
using Fims.Data.Models.Identity;

namespace Fims.Services.Identity
{
    public class IdentityService : IIdentityService
    {
        private const string InvalidErrorMessage = "Invalid email or password.";

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
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email
            };

            var identityResult = await this.userManager.CreateAsync(user, model.Password);

            var errors = identityResult.Errors.Select(e => e.Description);

            return identityResult.Succeeded
                ? Result.Success
                : Result.Failure(errors);
        }

        public async Task<Result<LoginResponseModel>> LoginAsync(LoginRequestModel model)
        {
            var user = await this.userManager.FindByEmailAsync(model.Email);
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

        public async Task<Result> ChangeUserProfileAsync(ChangeUserProfileRequestModel model, string userId)
        {
            var user = await this.userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return InvalidErrorMessage;
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            var identityResult = await this.userManager.UpdateAsync(user);

            var errors = identityResult.Errors.Select(e => e.Description);

            return identityResult.Succeeded
                ? Result.Success
                : Result.Failure(errors);
        }

        public async Task<Result> ChangePasswordAsync(ChangePasswordRequestModel model, string userId)
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
                    Email = user.Email,
                    Password = "************",
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = userRole,
                };
                userAuthInfos.Add(userAuthInfo);
            }

            return userAuthInfos;
        }


        public async Task<List<FimsRole>> Roles()
        {
            var roles = roleManager.Roles;
            var roleslist = roles.ToList();
            return roleslist;
        }
    }
}
