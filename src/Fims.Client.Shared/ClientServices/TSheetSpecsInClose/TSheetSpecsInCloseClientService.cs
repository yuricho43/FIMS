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

namespace Fims.Client.Shared.ClientServices.TSheetSpecsInClose
{
    public class TSheetSpecsInCloseClientService : ITSheetSpecsInCloseClientService
    {
        private readonly HttpClient http;

        private const string SaveTSheetSpecsInCloseByUserPath = "api/tsheetspecsinclose/SaveTSheetSpecsInCloseByUser";
        private const string GetTSheetSpecsInCloseByUserPath = "api/tsheetspecsinclose";
        private const string DeleteTSheetSpecsInCloseByUserIdProductSerialPath = "api/tsheetspecsinclose/DeleteTSheetSpecsInCloseBySerial";

        public TSheetSpecsInCloseClientService(HttpClient http)
        {
            this.http = http;
        }

        public async Task<string> SaveTSheetSpecsInCloseByUser(TSheetSpecsInProgressDto tSheetSpecsInCloseReqeust)
        {
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(200));
            string Message = null;
            string fileName = null;

            try
            {
                var response = await this.http.PostAsJsonAsync(SaveTSheetSpecsInCloseByUserPath, tSheetSpecsInCloseReqeust, source.Token);
                if (source?.IsCancellationRequested == false)
                {
                    fileName = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception e)
            {
                Message = (source?.IsCancellationRequested == true) ? "Request to API timed out" : e.Message;
                Console.WriteLine(Message);
                //_logger.LogError(e, "couldn't retrieve forecast");
            }
            finally
            {
                source = null;
                // poke blazor to reset 
                // in case an error has occurred
                //StateHasChanged();
            }

            return fileName;
        }

        public async Task<TSheetSpecsInProgressDto> GetTSheetSpecsInCloseByUser(string userId)
        {
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(200));
            string Message = null;
            TSheetSpecsInProgressDto tSheetSpecsInCloseDto = null;

            try
            {
                var response = await this.http.GetAsync($"{GetTSheetSpecsInCloseByUserPath}/{userId}", source.Token);
                if (source?.IsCancellationRequested == false)
                {
                    tSheetSpecsInCloseDto = await response.Content.ReadFromJsonAsync<TSheetSpecsInProgressDto>();
                }
            }
            catch (Exception e)
            {
                Message = (source?.IsCancellationRequested == true) ? "Request to API timed out" : e.Message;
                Console.WriteLine(Message);
                tSheetSpecsInCloseDto = new TSheetSpecsInProgressDto();
                tSheetSpecsInCloseDto.UserId = $"HTTPFAIL: {Message}"; //mark as FAIL
            }
            finally
            {
                source = null;
                // poke blazor to reset 
                // in case an error has occurred
                //StateHasChanged();
            }

            return tSheetSpecsInCloseDto;
        }

        public async Task<string> DeleteTSheetSpecsInCloseByUserIdProductSerial(string productSerial)
        {
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            HttpResponseMessage response = null;
            string Message = null;
            string deletedProductSerial = string.Empty;

            try
            {
                var result = await this.http.DeleteAsync($"{DeleteTSheetSpecsInCloseByUserIdProductSerialPath}/{productSerial}", source.Token);
                if (source?.IsCancellationRequested == false)
                {
                    response = result;
                    deletedProductSerial = await response.Content.ReadAsStringAsync(); ;
                }
            }
            catch (Exception e)
            {
                Message = (source?.IsCancellationRequested == true) ? "Request to API timed out" : e.Message;
                Console.WriteLine(Message);
                //_logger.LogError(e, "couldn't retrieve forecast");
            }
            finally
            {
                source = null;
                // poke blazor to reset 
                // in case an error has occurred
                //StateHasChanged();
            }

            return deletedProductSerial;
        }

    }
}
