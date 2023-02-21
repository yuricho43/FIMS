using Fims.Data.Entities;
using Fims.Data.Models;
using Fims.Data.Models.TSheets;


namespace Fims.Client.Shared.ClientServices.TSheets
{
    public interface ITSheetsClientService
    {
        Task<int> CreateTSheet(TSheet tSheet);

        Task<Result> UpdateTSheet(int id, TSheet tSheet);

        Task<Result> DeleteTSheet(int id);

        Task<IEnumerable<TSheet>> AllTSheetsAsync();

        Task<TSheet> FindTSheetWithTItems(int id);

        Task<TSheetsComplexSearchResponseModel> ComplexSearchAsync(TSheetsComplexSearchRequestModel searchRequest);
    }
}
