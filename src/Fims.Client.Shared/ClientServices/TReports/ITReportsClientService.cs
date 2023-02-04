using Fims.Data.Entities;
using Fims.Data.Models;
using Fims.Data.Models.TReports;


namespace Fims.Client.Shared.ClientServices.TReports
{
    public interface ITReportsClientService
    {
        Task<List<string>> AllTReportSpecs();
        Task<TReportDto> GenerateTReport(TReportDto tReportRequest);

        /*
        Task<Result> UpdateTSheet(int id, TSheet tSheet);

        Task<Result> RemoveTSheet(int id);

        Task<IEnumerable<TSheet>> AllTReportsAsync();

        Task<TSheet> FindTSheetByIdAsync(int id);
        Task<TSheet> FindTSheetWithDetailsByIdAsync(int id);
        */

    }
}
