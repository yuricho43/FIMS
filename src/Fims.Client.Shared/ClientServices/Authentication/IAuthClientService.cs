using System.Threading.Tasks;

using Fims.Data.Models;
using Fims.Data.Models.Identity;


namespace Fims.Client.Shared.ClientServices.Authentication
{
    public interface IAuthClientService
    {
        Task<Result> Register(RegisterRequestModel model);

        Task<Result> Login(LoginRequestModel model);

        Task Logout();

        Task<List<UserAuthInfoModel>> AllUsers();

    }
}
