using Fims.Data.Models.TSheetSpecsInProgress;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Fims.Services.TReports
{
    //JBH: Make sure NOT inherits IService|ISingletonService|IScopedService
    //     Instantiate TSheetSpecsInProgressService and AddSingleton() in ConfigureServices @ Startup.cs.
    //     This way, TSheetSpecsInProgressService will build TSheetSpecs immediatley upon startup.

    //public interface ITSheetSpecsInProgressService : ISingletonService
    public interface ITReportsService //DO NOT inherits IService|ISingletonService|IScopedService
    {
        public Task<string> SaveTSheetSpecsInProgressByUserAsync(string userId, TSheetSpecsInProgressDto tSheetSpecsInProgressDto);
        public Task<TSheetSpecsInProgressDto> GetTSheetSpecsInProgressAsync(string userId);
        public string DeleteTSheetSpecsInProgressByProductSerial(string productSerial);
    }
}
