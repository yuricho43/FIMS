using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using Fims.Client.Shared.Infrastructure.Extensions;
using Fims.Data.Entities;
using Fims.Data.Models;
using Fims.Data.Models.TSheets;


namespace Fims.Client.Shared.ClientServices.TSheets
{
    public class TSheetsClientService : ITSheetsClientService
    {
        private readonly HttpClient http;

        private const string TSheetsPath = "api/tsheets";
        private const string TSheetsSearchPath = TSheetsPath + "?customer={0}&minDateTime={1}&maxDateTime={2}&model={3}&page={4}";

        public TSheetsClientService(HttpClient http)
        {
            this.http = http;
        }

        public async Task<int> AddTSheet(TSheet tSheet)
        {
            var path = $"{TSheetsPath}/{nameof(this.AddTSheet)}";
            var tSheetResponse = await this.http.PostAsJsonAsync($"{TSheetsPath}/{nameof(this.AddTSheet)}", tSheet);
            var tSheetId = await tSheetResponse.Content.ReadFromJsonAsync<int>();
            //var tSheetId = 7;
            return tSheetId;
        }

        public async Task<Result> UpdateTSheet(int id, TSheet tSheet)
            => await this.http
                .PutAsJsonAsync($"{TSheetsPath}/{nameof(this.UpdateTSheet)}/{id}", tSheet)
                .ToResult();

        public async Task<Result> RemoveTSheet(int id)
            => await this.http.DeleteAsync($"{TSheetsPath}/{nameof(this.RemoveTSheet)}/{id}").ToResult();

        public async Task<IEnumerable<TSheet>> AllTSheetsAsync()
        {
            var response = await this.http.GetFromJsonAsync<IEnumerable<TSheet>>(TSheetsPath);
            return response;
        }

        // public async Task<TSheet> FindTSheetByIdAsync(int id)
        // {
        //     var response = await this.http.GetFromJsonAsync<TSheet>($"{TSheetsPath}/{nameof(this.FindTSheetByIdAsync)}/{id}");
        //     return response;
        // }

        public async Task<TSheet> FindTSheetWithDetailsByIdAsync(int id)
        {
            var response = await this.http.GetFromJsonAsync<TSheet>($"{TSheetsPath}/FindTSheetWithTItems/{id}");
            return response;
        }

        public async Task<TSheetsComplexSearchResponseModel> ComplexSearchAsync(TSheetsComplexSearchRequestModel searchRequest)
        {
            var searchResponse = await this.http.GetFromJsonAsync<TSheetsComplexSearchResponseModel>(
                string.Format(
                    TSheetsSearchPath,
                    searchRequest.Customer,
                    searchRequest.MinDateTime,
                    searchRequest.MaxDateTime,
                    searchRequest.Model,
                    searchRequest.Page));

            return searchResponse;
        }
    }
}
