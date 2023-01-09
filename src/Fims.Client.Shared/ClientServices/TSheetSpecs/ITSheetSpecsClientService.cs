using Fims.Data.Models.TSheetSpecs;


namespace Fims.Client.Shared.ClientServices.TSheetSpecs
{
    public interface ITSheetSpecsClientService
    {
        public Task<List<string>> GetEquipmentModelsAsync();
        public Task<Dictionary<string, TSheetSpec>> GetTSheetSpecsDictAsync();
        public Task<TSheetSpec> GetTSheetSpecByEquipmentModelAsync(string equipmentModel);

        /*
        Task<Result> AddProduct(TSheetRequest model);
        Task<Result> UpdateProduct(TSheetRequest model);
        Task<Result> RemoveProduct(int id);
        Task<int> TotalProductsCount();
        Task<IEnumerable<TSheetResponse>> Mine();
        */
    }
}
