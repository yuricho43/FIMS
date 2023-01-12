using System.Threading.Tasks;

using Fims.Common;
using Fims.Data.Models;
using Fims.Data.Models.Identity;
using Fims.Services.Common;


namespace Fims.Services.Identity
{

    public interface IIdentityService : IService
    {
        Task<Result> RegisterAsync(RegisterRequestModel model);

        Task<Result<LoginResponseModel>> LoginAsync(LoginRequestModel model);

        Task<Result> ChangeUserProfileAsync(ChangeUserProfileRequestModel model, string userId);

        Task<Result> ChangePasswordAsync(ChangePasswordRequestModel model, string userId);
    }
}
