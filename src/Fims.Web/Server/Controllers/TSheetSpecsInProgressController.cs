using System.Collections.Generic;
using System.Threading.Tasks;
using System;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Fims.Data.Models.TSheetSpecs;
using Fims.Services.TSheetSpecsInProgress;

using Fims.Web.Server.Infrastructure.Services;
using Fims.Data.Models.TSheetSpecsInProgress;
using Telerik.SvgIcons;

namespace Fims.Web.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TSheetSpecsInProgressController : ControllerBase
    {
        private readonly ITSheetSpecsInProgressService TSheetSpecsInProgressService;
        private readonly ICurrentUserService CurrentUserService;
        private readonly ILogger<TSheetSpecsInProgressController> logger;

        public TSheetSpecsInProgressController(
            ITSheetSpecsInProgressService tSheetSpecsSaveService,
            ICurrentUserService currentUserService,
            ILogger<TSheetSpecsInProgressController> logger)
        {
            this.TSheetSpecsInProgressService = tSheetSpecsSaveService;
            this.CurrentUserService = currentUserService;
            this.logger = logger;
        }


        [Authorize]
        [HttpGet("{userId}")]
        public async Task<ActionResult> GetTSheetSpecsInProgressByUser(string userId)
        {
            //JBH FIXME4DEBUG     SerilogExceptionHandlingMiddleware     throw new Exception("GetTSheetSpecsInProgressByUser: Failed to retrieve data");

            // "userId" should be same with "this.CurrentUserService.UserId", and unused now.
            logger.LogInformation($"fetch TSheets (in progress) for: {userId}");
            var tSheetSpecsInProgressDto = await this.TSheetSpecsInProgressService.GetTSheetSpecsInProgressAsync(this.CurrentUserService.UserId ?? "ANONYMOUS");
            var numTSheets = tSheetSpecsInProgressDto.SerialToTSheetSpecPairs.Count;
            return Created(nameof(this.GetTSheetSpecsInProgressByUser), tSheetSpecsInProgressDto);
        }


        [Authorize]
        [HttpPost(nameof(SaveTSheetSpecsInProgressByUser))]
        public async Task<ActionResult> SaveTSheetSpecsInProgressByUser(TSheetSpecsInProgressDto tSheetSpecsInProgressDto)
        {
            // "userId" should be same with "this.CurrentUserService.UserId", and unused now.
            var userId = tSheetSpecsInProgressDto.UserId;
            var numTSheets = tSheetSpecsInProgressDto.SerialToTSheetSpecPairs.Count;
            logger.LogInformation($"save {numTSheets} TSheets (in progress) for: {tSheetSpecsInProgressDto.UserName} ({userId})");
            var fileName = await this.TSheetSpecsInProgressService.SaveTSheetSpecsInProgressByUserAsync(this.CurrentUserService.UserId ?? "ANONYMOUS", tSheetSpecsInProgressDto);
            return Created(nameof(this.SaveTSheetSpecsInProgressByUser), userId);
        }


        [Authorize]
        [HttpDelete("DeleteTSheetSpecsInProgressBySerial/{productSerial}")]
        public string DeleteTSheetSpecsInProgressBySerial(string productSerial)
        {
            logger.LogInformation($"delete TSheet (in progress) of: {productSerial}");
            var deletedProductSerial = this.TSheetSpecsInProgressService.DeleteTSheetSpecsInProgressByProductSerial(productSerial);
            return deletedProductSerial;
        }
    }
}
