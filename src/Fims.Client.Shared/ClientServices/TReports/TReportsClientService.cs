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

        private const string AllTReportSpecsPath = "api/treports/AllTReportSpecs";
        private const string GenerateTReportPath = "api/treports/GenerateTReport";
        private const string UploadSpecFilePath  = "api/treports/UploadSpecFile";

        public TReportsClientService(HttpClient http)
        {
            this.http = http;
        }

        public async Task<List<string>> AllTReportSpecs()
        {
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            HttpResponseMessage response = null;
            string Message = null;
            List<string> tReportSpecs = new List<string>();

            try
            {
                var result = await this.http.GetFromJsonAsync<List<string>>(AllTReportSpecsPath, source.Token);
                if (source?.IsCancellationRequested == false)
                {
                    tReportSpecs = result;
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

            return tReportSpecs;
        }

        public async Task<HttpResponseMessage> GenerateTReport(TReportDto tReportRequest)
        {
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            HttpResponseMessage response = null;
            string Message = null;

            try
            {
                var result = await this.http.PostAsJsonAsync(GenerateTReportPath, tReportRequest, source.Token);
                if (source?.IsCancellationRequested == false)
                {
                    response = result;
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

            return response;
            //tReportRequest.IsSuccess = true;
            //return tReportRequest;
            ////var tReportResponse = await response.Content.ReadFromJsonAsync<TReportDto>();
            ////return tReportResponse;
        }


        public async Task<string> UploadSpecFile(MultipartFormDataContent content)
        {
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            string Message = null;
            string uploadResult = string.Empty;

            try
            {
                var response = await http.PostAsync(UploadSpecFilePath, content);
                if (source?.IsCancellationRequested == false)
                {
                    uploadResult = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception e)
            {
                Message = (source?.IsCancellationRequested == true) ? "Request to API timed out" : e.Message;
                uploadResult = "NOSERVER: " + Message;
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

            return uploadResult;
        }

    }
}
