using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using Fims.Client.Shared.Infrastructure.Extensions;
using Fims.Data.Entities;
using Fims.Data.Models;
using Fims.Data.Models.Identity;
using Fims.Data.Models.TSheets;


namespace Fims.Client.Shared.ClientServices.TSheets
{
    public class TSheetsClientService : ITSheetsClientService
    {
        private readonly HttpClient http;

        private const string AllTSheetsPath = "api/tsheets";
        private const string CreateTSheetPath = "api/tsheets/CreateTSheet";
        private const string UpdateTSheetPath = "api/tsheets/UpdateTSheet";
        private const string DeleteTSheetPath = "api/tsheets/DeleteTSheet";
        private const string FindTSheetWithTItemsPath = "api/tsheets/FindTSheetWithTItems";
        private const string TSheetsSearchPath = "api/tsheets?customer={0}&minDateTime={1}&maxDateTime={2}&model={3}&page={4}";

        public TSheetsClientService(HttpClient http)
        {
            this.http = http;
        }

        public async Task<int> CreateTSheet(TSheet tSheet, int itype)
        {
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            HttpResponseMessage response = null;
            string Message = null;
            int tSheetId = 0;

            try
            {
                string newPath = CreateTSheetPath + "?itype="+itype.ToString();
                // var result = await this.http.PostAsJsonAsync(CreateTSheetPath, tSheet, source.Token);
                var result = await this.http.PostAsJsonAsync(newPath, tSheet, source.Token);
                if (source?.IsCancellationRequested == false)
                {
                    response = result;
                    tSheetId = await response.Content.ReadFromJsonAsync<int>();
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

            return tSheetId;
        }

        public async Task<Result> UpdateTSheet(int id, TSheet tSheet)
        {
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            string Message = null;
            HttpResponseMessage response = null;
            List<string> errors = new List<string>();

            try
            {
                var result = await this.http.PutAsJsonAsync($"{UpdateTSheetPath}/{id}", tSheet, source.Token);
                if (source?.IsCancellationRequested == false)
                {
                    response = result;
                    if (!response.IsSuccessStatusCode)
                    {
                        errors = await response.Content.ReadFromJsonAsync<List<string>>();
                    }
                }
            }
            catch (Exception e)
            {
                Message = (source?.IsCancellationRequested == true) ? "Request to API timed out" : e.Message;
                Console.WriteLine(Message);
                errors.Add("서버 연결 실패!");
                //_logger.LogError(e, "couldn't retrieve forecast");
            }
            finally
            {
                source = null;
                // poke blazor to reset 
                // in case an error has occurred
                //StateHasChanged();
            }

            if (errors.Count > 0)
            {
                return Result.Failure(errors);
            }
            else
            {
                return Result.Success;
            }
        }

        public async Task<Result> DeleteTSheet(int id)
        {
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            string Message = null;
            HttpResponseMessage response = null;
            List<string> errors = new List<string>();

            try
            {
                var result = await this.http.DeleteAsync($"{DeleteTSheetPath}/{id}", source.Token);
                if (source?.IsCancellationRequested == false)
                {
                    response = result;
                    if (!response.IsSuccessStatusCode)
                    {
                        errors = await response.Content.ReadFromJsonAsync<List<string>>();
                    }
                }
            }
            catch (Exception e)
            {
                Message = (source?.IsCancellationRequested == true) ? "Request to API timed out" : e.Message;
                Console.WriteLine(Message);
                errors.Add("서버 연결 실패!");
                //_logger.LogError(e, "couldn't retrieve forecast");
            }
            finally
            {
                source = null;
                // poke blazor to reset 
                // in case an error has occurred
                //StateHasChanged();
            }

            if (errors.Count > 0)
            {
                return Result.Failure(errors);
            }
            else
            {
                return Result.Success;
            }
        }

        public async Task<IEnumerable<TSheet>> AllTSheetsAsync()
        {
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            string Message = null;
            IEnumerable<TSheet> tSheets = Enumerable.Empty<TSheet>();

            try
            {
                var result = await this.http.GetFromJsonAsync<IEnumerable<TSheet>>(AllTSheetsPath, source.Token);
                if (source?.IsCancellationRequested == false)
                {
                    tSheets = result;
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

            return tSheets;
        }

        public async Task<TSheet> FindTSheetWithTItems(int id)
        {
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            string Message = null;
            TSheet tSheetWithTItems = null;

            try
            {
                var result = await this.http.GetFromJsonAsync<TSheet>($"{FindTSheetWithTItemsPath}/{id}", source.Token);
                if (source?.IsCancellationRequested == false)
                {
                    tSheetWithTItems = result;
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

            return tSheetWithTItems;
        }

        public async Task<TSheetsComplexSearchResponseModel> ComplexSearchAsync(TSheetsComplexSearchRequestModel searchRequest)
        {
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            string Message = null;
            TSheetsComplexSearchResponseModel searchResponse = null;

            try
            {
                var requestPath = string.Format(TSheetsSearchPath, searchRequest.Customer, searchRequest.MinDateTime, searchRequest.MaxDateTime, searchRequest.Model, searchRequest.Page);
                var result = await this.http.GetFromJsonAsync<TSheetsComplexSearchResponseModel>(requestPath, source.Token);
                if (source?.IsCancellationRequested == false)
                {
                    searchResponse = result;
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

            return searchResponse;
        }
    }
}
