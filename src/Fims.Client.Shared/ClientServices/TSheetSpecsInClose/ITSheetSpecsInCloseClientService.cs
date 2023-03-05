using Fims.Data.Entities;
using Fims.Data.Models;
using Fims.Data.Models.TSheets;
using Fims.Data.Models.TSheetSpecsInProgress;

namespace Fims.Client.Shared.ClientServices.TSheetSpecsInClose
{
    public interface ITSheetSpecsInCloseClientService
    {
        Task<string> SaveTSheetSpecsInCloseByUser(TSheetSpecsInProgressDto tSheetSpecsInCloseDto);
        Task<TSheetSpecsInProgressDto> GetTSheetSpecsInCloseByUser(string userId);
        Task<string> DeleteTSheetSpecsInCloseByUserIdProductSerial(string productSerial);
    }
}
