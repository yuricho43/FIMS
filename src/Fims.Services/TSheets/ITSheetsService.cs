using System.Collections.Generic;
using System.Threading.Tasks;

using Fims.Data.Entities;
using Fims.Data.Models;
using Fims.Data.Models.TSheets;
using Fims.Services.Common;


namespace Fims.Services.TSheets
{
    public interface ITSheetsService : IService
    {
        Task<int> CreateAsync(TSheet tSheet, string userId);

        Task<Result> UpdateAsync(int id, TSheet newTSheet, string userId);

        Task<Result> DeleteAsync(int id);

        //Task<TSheet> DetailsAsync(int id);

        Task<TSheetsComplexSearchResponseModel> ComplexSearchAsync(TSheetsComplexSearchRequestModel searchRequest);

        Task<IEnumerable<TSheet>> AllTSheetsAsync();

        Task<TSheet> FindTSheetWithTItemsByIdAsync(int id);
    }
}