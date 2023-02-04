using Fims.Data.Models.TReports;
using Fims.Data.Models.TSheetSpecsInProgress;
using Fims.Services.Common;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Fims.Services.TReports
{
    public interface ITReportsService : IService
    {
        Task<List<string>> AllTReportSpecsAsync();

        Task<TReportDto> GenerateTReportAsync(TReportDto tReportRequest);

        // public Task<string> SaveTSheetSpecsInProgressByUserAsync(string userId, TSheetSpecsInProgressDto tSheetSpecsInProgressDto);
        // public Task<TSheetSpecsInProgressDto> GetTSheetSpecsInProgressAsync(string userId);
        // public string DeleteTSheetSpecsInProgressByProductSerial(string productSerial);
    }
}
