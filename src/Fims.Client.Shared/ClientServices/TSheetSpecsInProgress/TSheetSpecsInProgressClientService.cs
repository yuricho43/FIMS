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

        private const string SaveTSheetSpecsInProgressByUserPath = "api/tsheetspecsinprogress/SaveTSheetSpecsInProgressByUser";
        private const string GetTSheetSpecsInProgressByUserPath = "api/tsheetspecsinprogress";
        private const string DeleteTSheetSpecsInProgressByUserIdProductSerialPath = "api/tsheetspecsinprogress/DeleteTSheetSpecsInProgressBySerial";

        public TSheetSpecsInProgressClientService(HttpClient http)
        {
            this.http = http;
        }

        public async Task<string> SaveTSheetSpecsInProgressByUser(TSheetSpecsInProgressDto tSheetSpecsInProgressReqeust)
        {
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(25));
            string Message = null;
            string fileName = null;

            try
            {
                var response = await this.http.PostAsJsonAsync(SaveTSheetSpecsInProgressByUserPath, tSheetSpecsInProgressReqeust, source.Token);
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

        public async Task<TSheetSpecsInProgressDto> GetTSheetSpecsInProgressByUser(string userId)
        {
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(25));
            string Message = null;
            TSheetSpecsInProgressDto tSheetSpecsInProgressDto = null;

            try
            {
                var response = await this.http.GetAsync($"{GetTSheetSpecsInProgressByUserPath}/{userId}", source.Token);
                if (source?.IsCancellationRequested == false)
                {
                    tSheetSpecsInProgressDto = await response.Content.ReadFromJsonAsync<TSheetSpecsInProgressDto>();
                }
            }
            catch (Exception e)
            {
                Message = (source?.IsCancellationRequested == true) ? "Request to API timed out" : e.Message;
                Console.WriteLine(Message);
                tSheetSpecsInProgressDto = new TSheetSpecsInProgressDto();
                tSheetSpecsInProgressDto.UserId = $"HTTPFAIL: {Message}"; //mark as FAIL
            }
            finally
            {
                source = null;
                // poke blazor to reset 
                // in case an error has occurred
                //StateHasChanged();
            }

            return tSheetSpecsInProgressDto;
        }

        public async Task<string> DeleteTSheetSpecsInProgressByUserIdProductSerial(string productSerial)
        {
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            HttpResponseMessage response = null;
            string Message = null;
            string deletedProductSerial = string.Empty;

            try
            {
                var result = await this.http.DeleteAsync($"{DeleteTSheetSpecsInProgressByUserIdProductSerialPath}/{productSerial}", source.Token);
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
