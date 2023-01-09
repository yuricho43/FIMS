using System.Threading.Tasks;

using Fims.Data.Entities;
using Fims.Services.Common;


namespace Fims.Services.Identity
{
    public interface IJwtGeneratorService : IService
    {
        Task<string> GenerateJwtAsync(FimsUser user);
    }
}
