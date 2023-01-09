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

        public async Task<List<string>> GetEquipmentModelsAsync()
        {
            // GET: api/TSheetSpecs/TSheetModels
            var equipmentModels = await this.http.GetFromJsonAsync<List<string>>(TSheetSpecsRoute + "/TSheetModels");
            return equipmentModels;
        }

        public async Task<Dictionary<string, TSheetSpec>> GetTSheetSpecsDictAsync()
        {
            // GET: api/TSheetSpecs/TSheetSpecsDict
            var tSheetSpecsDict = await this.http.GetFromJsonAsync<Dictionary<string, TSheetSpec>>(TSheetSpecsRoute + "/TSheetSpecsDict");
            return tSheetSpecsDict;
        }

        public async Task<TSheetSpec> GetTSheetSpecByEquipmentModelAsync(string equipmentModel)
        {
            // GET: api/TSheetSpecs/TSheetSpecByModel/{equipmentModel}
            var tSheetSpec = await this.http.GetFromJsonAsync<TSheetSpec>(TSheetSpecsRoute + "/TSheetSpecByModel/" + equipmentModel);
            return tSheetSpec;
        }


        /*
        public async Task<Result> AddProduct(TSheetRequest model)
            => await this.http
                .PostAsJsonAsync($"{TSheetSpecsPath}/{nameof(this.AddProduct)}", model)
                .ToResult();

        public async Task<Result> UpdateProduct(TSheetRequest model)
            => await this.http
                .PutAsJsonAsync($"{TSheetSpecsPath}/{nameof(this.UpdateProduct)}", model)
                .ToResult();

        public async Task<Result> RemoveProduct(int id)
            => await this.http.DeleteAsync($"{TSheetSpecsPath}/{nameof(this.RemoveProduct)}/{id}").ToResult();

        public async Task<int> TotalProductsCount()
            => await this.http.GetFromJsonAsync<int>($"{TSheetSpecsPath}/{nameof(this.TotalProductsCount)}");

        public async Task<IEnumerable<TSheetResponse>> Mine()
            => await this.http.GetFromJsonAsync<IEnumerable<TSheetResponse>>(TSheetSpecsPath);
        */
    }
}
