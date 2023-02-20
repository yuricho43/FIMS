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
using Telerik.SvgIcons;

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
            string fileName = null;
            try
            {
                var path = $"{TSheetSpecsInProgressPath}/{nameof(this.SaveTSheetSpecsInProgressByUser)}";
                var tSheetSpecsInProgressResponse = await this.http.PostAsJsonAsync($"{TSheetSpecsInProgressPath}/{nameof(this.SaveTSheetSpecsInProgressByUser)}", tSheetSpecsInProgressReqeust);
                fileName = await tSheetSpecsInProgressResponse.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                //no connection to the server
                Console.WriteLine(ex.Message);
            }
            return fileName;
        }

        public async Task<TSheetSpecsInProgressDto> GetTSheetSpecsInProgressByUser(string userId)
        {
            TSheetSpecsInProgressDto tSheetSpecsInProgressDto;
            try
            {
                var tSheetSpecsInProgressResponse = await this.http.GetAsync(TSheetSpecsInProgressPath + "/" + userId);
                tSheetSpecsInProgressDto = await tSheetSpecsInProgressResponse.Content.ReadFromJsonAsync<TSheetSpecsInProgressDto>();
            }
            catch (Exception ex)
            {
                //no connection to the server
                Console.WriteLine(ex.Message);
                tSheetSpecsInProgressDto = new TSheetSpecsInProgressDto();
                tSheetSpecsInProgressDto.UserId = $"HTTPFAIL: {ex.Message}";
            }
            return tSheetSpecsInProgressDto;
        }

        public async Task<string> DeleteTSheetSpecsInProgressByUserIdProductSerial(string productSerial)
        {
            var path = $"{TSheetSpecsInProgressPath}/DeleteTSheetSpecsInProgressBySerial/{productSerial}";
            var tSheetSpecsInProgressResponse = await this.http.DeleteAsync(path);
            var deletedProductSerial = await tSheetSpecsInProgressResponse.Content.ReadAsStringAsync();
            return deletedProductSerial;
        }

    }
}
