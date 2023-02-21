using Fims.Data.Models.TSheetSpecs;


namespace Fims.Client.Shared.ClientServices.TSheetSpecs
{
    public interface ITSheetSpecsClientService
    {
        public Task<List<string>> GetEquipmentModelsAsync();
        public Task<Dictionary<string, TSheetSpec>> GetTSheetSpecsDictAsync();
        public Task<TSheetSpec> GetTSheetSpecByEquipmentModelAsync(string equipmentModel);
        public Task<string> UploadSpecFile(MultipartFormDataContent content);
    }
}
