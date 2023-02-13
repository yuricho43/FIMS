using Fims.Data.Entities;
using Fims.Data.Models;
using Fims.Data.Models.TSheets;


namespace Fims.Client.Shared.ClientServices.TSheets
{
    public interface ITSheetsClientService
    {
        Task<int> CreateTSheet(TSheet tSheet);

        Task<Result> UpdateTSheet(int id, TSheet tSheet);

        Task<Result> RemoveTSheet(int id);

        Task<IEnumerable<TSheet>> AllTSheetsAsync();

        // Task<TSheet> FindTSheetByIdAsync(int id);
        Task<TSheet> FindTSheetWithDetailsByIdAsync(int id);

        Task<TSheetsComplexSearchResponseModel> ComplexSearchAsync(TSheetsComplexSearchRequestModel searchRequest);
    }
}
