using System.Threading.Tasks;
using Fims.Data.Entities;
using Fims.Data.Models;
using Fims.Data.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace Fims.Client.Shared.ClientServices.Authentication
{
    public interface IAuthClientService
    {
        Task<Result> Register(RegisterRequestModel model);

        Task<Result> Login(LoginRequestModel model);

        Task Logout();

        Task<List<UserAuthInfoModel>> AllUsers();
        Task<List<FimsRole>> AllRoles();

        Task<Result> Delete(string username);

        Task<Result> ChangeRole(UserAuthInfoModel model);

        Task<Result> ChangeProfile(UserProfileModel model);

        Task<Result> ChangePassword(PasswordModel model);

        Task<Result> ResetPassword(UserAuthInfoModel model);
    }
}
