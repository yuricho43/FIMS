using Fims.Data.Entities;
using Fims.Data.Models;
using Fims.Data.Models.TReports;


namespace Fims.Client.Shared.ClientServices.TReports
{
    public interface ITReportsClientService
    {
        Task<List<string>> AllTReportSpecs();
        Task<HttpResponseMessage> GenerateTReport(TReportDto tReportRequest);
        public Task<string> UploadSpecFile(MultipartFormDataContent content);
    }
}
