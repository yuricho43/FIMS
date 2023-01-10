using Fims.Data.Entities;
using Fims.Data.Models;
using Fims.Data.Models.TSheets;
using Fims.Data.Models.TSheetSpecsInProgress;

namespace Fims.Client.Shared.ClientServices.TSheetSpecsInProgress
{
    public interface ITSheetSpecsInProgressClientService
    {
        Task<string> SaveTSheetSpecsInProgressByUser(TSheetSpecsInProgressDto tSheetSpecsInProgressDto);
        Task<TSheetSpecsInProgressDto> GetTSheetSpecsInProgressByUser(string userId);
    }
}
