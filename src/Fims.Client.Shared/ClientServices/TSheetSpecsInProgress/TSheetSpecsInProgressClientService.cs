using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

using Fims.Client.Shared.Infrastructure.Extensions;
using Fims.Data.Entities;
using Fims.Data.Models;
using Fims.Data.Models.TSheets;
using Fims.Data.Models.TSheetSpecs;
using Fims.Data.Models.TSheetSpecsInProgress;

namespace Fims.Client.Shared.ClientServices.TSheetSpecsInProgress
{
    public class TSheetSpecsInProgressClientService : ITSheetSpecsInProgressClientService
    {
        private readonly HttpClient http;

        private const string TSheetSpecsInProgressPath = "api/tsheetspecsinprogress";

        public TSheetSpecsInProgressClientService(HttpClient http)
        {
            this.http = http;
        }

        public async Task<string> SaveTSheetSpecsInProgressByUser(TSheetSpecsInProgressDto tSheetSpecsInProgressReqeust)
        {
            var path = $"{TSheetSpecsInProgressPath}/{nameof(this.SaveTSheetSpecsInProgressByUser)}";
            var tSheetSpecsInProgressResponse = await this.http.PostAsJsonAsync($"{TSheetSpecsInProgressPath}/{nameof(this.SaveTSheetSpecsInProgressByUser)}", tSheetSpecsInProgressReqeust);
            var fileName = await tSheetSpecsInProgressResponse.Content.ReadAsStringAsync();
            return fileName;
        }

        public async Task<TSheetSpecsInProgressDto> GetTSheetSpecsInProgressByUser(string userId)
        {
            var tSheetSpecsInProgressResponse = await this.http.GetAsync(TSheetSpecsInProgressPath + "/" + userId);
            var tSheetSpecsInProgressDto = await tSheetSpecsInProgressResponse.Content.ReadFromJsonAsync<TSheetSpecsInProgressDto>();
            return tSheetSpecsInProgressDto;
        }
    }
}
