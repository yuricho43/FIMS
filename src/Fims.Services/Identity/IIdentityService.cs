using System.Collections.Generic;
using System.Threading.Tasks;

using Fims.Common;
using Fims.Data.Entities;
using Fims.Data.Models;
using Fims.Data.Models.Identity;
using Fims.Services.Common;
using Microsoft.AspNetCore.Identity;

namespace Fims.Services.Identity
{

    public interface IIdentityService : IService
    {
        Task<Result> RegisterAsync(RegisterRequestModel model);

        Task<Result<LoginResponseModel>> LoginAsync(LoginRequestModel model);

        Task<Result> ChangeUserProfileAsync(UserProfileModel model, string userId);

        Task<Result> ChangePasswordAsync(PasswordModel model, string userId);

        Task<Result> ResetPasswordAsync(UserAuthInfoModel model);

        Task<List<UserAuthInfoModel>> AllUsers();
        List<FimsRole> Roles();

        Task<Result> DeleteAsync(string username);

    }
}
