using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Fims.Data.Models.TSheetSpecs;
using Fims.Services.TSheetSpecsInClose;

using Fims.Web.Server.Infrastructure.Services;
using Fims.Data.Models.TSheetSpecsInProgress;

namespace Fims.Web.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TSheetSpecsInCloseController : ControllerBase
    {
        private readonly ITSheetSpecsInCloseService TSheetSpecsInCloseService;
        private readonly ICurrentUserService CurrentUserService;
        private readonly ILogger<TSheetSpecsInCloseController> logger;

        public TSheetSpecsInCloseController(
            ITSheetSpecsInCloseService tSheetSpecsSaveService,
            ICurrentUserService currentUserService,
            ILogger<TSheetSpecsInCloseController> logger)
        {
            this.TSheetSpecsInCloseService = tSheetSpecsSaveService;
            this.CurrentUserService = currentUserService;
            this.logger = logger;
        }


        [Authorize]
        [HttpGet("{userId}")]
        public async Task<ActionResult> GetTSheetSpecsInCloseByUser(string userId)
        {
            // "userId" should be same with "this.CurrentUserService.UserId", and unused now.
            logger.LogInformation($"fetch TSheets (in close) for: {userId}");
            var tSheetSpecsInCloseDto = await this.TSheetSpecsInCloseService.GetTSheetSpecsInCloseAsync(this.CurrentUserService.UserId ?? "ANONYMOUS");
            var numTSheets = tSheetSpecsInCloseDto.SerialToTSheetSpecPairs.Count;
            return Created(nameof(this.GetTSheetSpecsInCloseByUser), tSheetSpecsInCloseDto);
        }


        [Authorize]
        [HttpPost(nameof(SaveTSheetSpecsInCloseByUser))]
        public async Task<ActionResult> SaveTSheetSpecsInCloseByUser(TSheetSpecsInProgressDto tSheetSpecsInCloseDto)
        {
            // "userId" should be same with "this.CurrentUserService.UserId", and unused now.
            var userId = tSheetSpecsInCloseDto.UserId;
            var numTSheets = tSheetSpecsInCloseDto.SerialToTSheetSpecPairs.Count;
            logger.LogInformation($"save {numTSheets} TSheets (in close) for: {tSheetSpecsInCloseDto.UserName} ({userId})");
            var fileName = await this.TSheetSpecsInCloseService.SaveTSheetSpecsInCloseAsync(this.CurrentUserService.UserId ?? "ANONYMOUS", tSheetSpecsInCloseDto);
            return Created(nameof(this.SaveTSheetSpecsInCloseByUser), userId);
        }


        [Authorize]
        [HttpDelete("DeleteTSheetSpecsInCloseBySerial/{productSerial}")]
        public string DeleteTSheetSpecsInCloseBySerial(string productSerial)
        {
            logger.LogInformation($"delete TSheet (in close) of: {productSerial}");
            var deletedProductSerial = this.TSheetSpecsInCloseService.DeleteTSheetSpecsInCloseByProductSerial(productSerial);
            return deletedProductSerial;
        }
    }
}
