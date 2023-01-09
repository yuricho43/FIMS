using System.Collections.Generic;
using System.Threading.Tasks;


namespace Fims.Services.TSheetSpecsInProgress
{
    //JBH: Make sure NOT inherits IService|ISingletonService|IScopedService
    //     Instantiate TSheetSpecsInProgressService and AddSingleton() in ConfigureServices @ Startup.cs.
    //     This way, TSheetSpecsInProgressService will build TSheetSpecs immediatley upon startup.

    //public interface ITSheetSpecsInProgressService : ISingletonService
    public interface ITSheetSpecsInProgressService //DO NOT inherits IService|ISingletonService|IScopedService
    {
        public Task<string> SaveTSheetSpecsInProgressByUserAsync(string tSheetSpecsJsonStr, string userId);
        public Task<string> GetTSheetSpecsInProgressAsync(string userId);
    }
}
