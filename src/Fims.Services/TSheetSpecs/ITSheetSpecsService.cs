using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using Fims.Data.Models.TSheetSpecs;


namespace Fims.Services.TSheetSpecs
{
    //JBH: Make sure NOT inherits IService|ISingletonService|IScopedService
    //     Instantiate TSheetSpecsService and AddSingleton() in ConfigureServices @ Startup.cs.
    //     This way, TSheetSpecsService will build TSheetSpecs immediatley upon startup.

    //public interface ITSheetSpecsService : ISingletonService
    public interface ITSheetSpecsService //DO NOT inherits IService|ISingletonService|IScopedService
    {
        public Task<List<string>> GetEquipmentModelsAsync();
        public Task<Dictionary<string, TSheetSpec>> GetTSheetSpecsDictAsync();
        public Task<TSheetSpec> GetTSheetSpecByEquipmentModelAsync(string equipmentModel);
        public Task<bool> SaveAsync(IEnumerable<IFormFile> files);
        public Task<bool> RemoveAsync(string[] files);
    }
}
