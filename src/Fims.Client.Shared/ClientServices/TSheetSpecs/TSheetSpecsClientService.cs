using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using Fims.Data.Models.TSheetSpecs;


namespace Fims.Client.Shared.ClientServices.TSheetSpecs
{
    public class TSheetSpecsClientService : ITSheetSpecsClientService
    {
        private readonly HttpClient http;

        private const string TSheetSpecsRoute = "api/TSheetSpecs";

        public TSheetSpecsClientService(HttpClient http)
        {
            this.http = http;
        }

        //public async Task<List<string>> GetEquipmentModelsAsync()
        //{
        //    // GET: api/TSheetSpecs/TSheetModels
        //    var equipmentModels = await this.http.GetFromJsonAsync<List<string>>(TSheetSpecsRoute + "/TSheetModels");
        //    return equipmentModels;
        //}
        public async Task<List<string>> GetEquipmentModelsAsync()
        {
            // set the member, not a local variable
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            string Message = null;
            List<string> equipmentModels = new List<string>();

            try
            {
                var result = await this.http.GetFromJsonAsync<List<string>>(TSheetSpecsRoute + "/TSheetModels", source.Token);
                if (source?.IsCancellationRequested == false)
                {
                    equipmentModels = result;
                }
            }
            catch (Exception e)
            {
                Message = source?.IsCancellationRequested == true
                    ? "Request to API timed out"
                    : e.Message;

                //_logger.LogError(e, "couldn't retrieve forecast");
            }
            finally
            {
                source = null;
                // poke blazor to reset 
                // in case an error has occurred
                //StateHasChanged();
            }
            return equipmentModels.ToList();
        }

        public async Task<Dictionary<string, TSheetSpec>> GetTSheetSpecsDictAsync()
        {
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            string Message = null;
            Dictionary<string, TSheetSpec> tSheetSpecsDict = null;

            try
            {
                // GET: api/TSheetSpecs/TSheetSpecByModel/{equipmentModel}
                var result = await this.http.GetFromJsonAsync<Dictionary<string, TSheetSpec>>(TSheetSpecsRoute + "/TSheetSpecsDict", source.Token);
                if (source?.IsCancellationRequested == false)
                {
                    tSheetSpecsDict = result;
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

            // GET: api/TSheetSpecs/TSheetSpecsDict
            return tSheetSpecsDict;
        }

        public async Task<TSheetSpec> GetTSheetSpecByEquipmentModelAsync(string equipmentModel)
        {
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            string Message = null;
            TSheetSpec tSheetSpec = null;

            try
            {
                // GET: api/TSheetSpecs/TSheetSpecByModel/{equipmentModel}
                var result = await this.http.GetFromJsonAsync<TSheetSpec>(TSheetSpecsRoute + "/TSheetSpecByModel/" + equipmentModel, source.Token);
                if (source?.IsCancellationRequested == false)
                {
                    tSheetSpec = result;
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

            return tSheetSpec;

        }
    }
}
