using Fims.Data.Models.TSheetSpecsInProgress;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Fims.Services.TSheetSpecsInClose
{
    //JBH: Make sure NOT inherits IService|ISingletonService|IScopedService
    //     Instantiate TSheetSpecsClosingService and AddSingleton() in ConfigureServices @ Startup.cs.
    //     This way, TSheetSpecsClosingService will build TSheetSpecs immediatley upon startup.

    //public interface ITSheetSpecsClosingService : ISingletonService
    public interface ITSheetSpecsInCloseService //DO NOT inherits IService|ISingletonService|IScopedService
    {
        public Task<string> SaveTSheetSpecsInCloseAsync(string userId, TSheetSpecsInProgressDto tSheetSpecsInCloseDto);
        public Task<TSheetSpecsInProgressDto> GetTSheetSpecsInCloseAsync(string userId);
        public string DeleteTSheetSpecsInCloseByProductSerial(string productSerial);
    }
}
