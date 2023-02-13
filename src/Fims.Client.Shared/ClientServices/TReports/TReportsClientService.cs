using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using Fims.Client.Shared.Infrastructure.Extensions;
using Fims.Data.Entities;
using Fims.Data.Models;
using Fims.Data.Models.TReports;


namespace Fims.Client.Shared.ClientServices.TReports
{
    public class TReportsClientService : ITReportsClientService
    {
        private readonly HttpClient http;

        private const string TReportsPath = "api/treports";

        public TReportsClientService(HttpClient http)
        {
            this.http = http;
        }

        public async Task<List<string>> AllTReportSpecs()
        {
            var path = $"{TReportsPath}/{nameof(this.AllTReportSpecs)}"; // "api/treports/AllTReportSpecs"
            var response = await this.http.GetFromJsonAsync<List<string>>(path);
            return response;
        }

        public async Task<HttpResponseMessage> GenerateTReport(TReportDto tReportRequest)
        {
            var path = $"{TReportsPath}/{nameof(this.GenerateTReport)}"; // "api/treports/GenerateTReport"
            var response = await this.http.PostAsJsonAsync(path, tReportRequest);
            return response;
            //tReportRequest.IsSuccess = true;
            //return tReportRequest;
            ////var tReportResponse = await response.Content.ReadFromJsonAsync<TReportDto>();
            ////return tReportResponse;
        }

        /*
        public async Task<Result> UpdateTSheet(int id, TSheet tSheet)
            => await this.http
                .PutAsJsonAsync($"{TReportsPath}/{nameof(this.UpdateTSheet)}/{id}", tSheet)
                .ToResult();

        public async Task<Result> RemoveTSheet(int id)
            => await this.http.DeleteAsync($"{TReportsPath}/{nameof(this.RemoveTSheet)}/{id}").ToResult();

        public async Task<IEnumerable<TSheet>> AllTReportsAsync()
        {
            var response = await this.http.GetFromJsonAsync<IEnumerable<TSheet>>(TReportsPath);
            return response;
        }

        public async Task<TSheet> FindTSheetByIdAsync(int id)
        {
            var response = await this.http.GetFromJsonAsync<TSheet>($"{TReportsPath}/{nameof(this.FindTSheetByIdAsync)}/{id}");
            return response;
        }

        public async Task<TSheet> FindTSheetWithDetailsByIdAsync(int id)
        {
            var response = await this.http.GetFromJsonAsync<TSheet>($"{TReportsPath}/FindTSheetWithTItems/{id}");
            return response;
        }
        */
    }
}
