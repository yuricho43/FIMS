using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Fims.Data.Models.TSheetSpecs;
using Fims.Services.TSheetSpecsInProgress;

using Fims.Web.Server.Infrastructure.Services;
using Fims.Data.Models.TSheetSpecsInProgress;

namespace Fims.Web.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TSheetSpecsInProgressController : ControllerBase
    {
        private readonly ITSheetSpecsInProgressService TSheetSpecsInProgressService;
        private readonly ICurrentUserService CurrentUserService;

        public TSheetSpecsInProgressController(
            ITSheetSpecsInProgressService tSheetSpecsSaveService,
            ICurrentUserService currentUserService)
        {
            this.TSheetSpecsInProgressService = tSheetSpecsSaveService;
            this.CurrentUserService = currentUserService;
        }


        [HttpGet("{userId}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetTSheetSpecsInProgressByUser(string userId)
        {
            // "userId" should be same with "this.CurrentUserService.UserId", and unused now.
            var tSheetSpecsInProgressDto = await this.TSheetSpecsInProgressService.GetTSheetSpecsInProgressAsync(this.CurrentUserService.UserId ?? "ANONYMOUS");
            return Created(nameof(this.GetTSheetSpecsInProgressByUser), tSheetSpecsInProgressDto);
        }


        [HttpPost(nameof(SaveTSheetSpecsInProgressByUser))]
        [AllowAnonymous]
        public async Task<ActionResult> SaveTSheetSpecsInProgressByUser(TSheetSpecsInProgressDto tSheetSpecsInProgressDto)
        {
            // "userId" should be same with "this.CurrentUserService.UserId", and unused now.
            var userId = tSheetSpecsInProgressDto.UserId;
            var fileName = await this.TSheetSpecsInProgressService.SaveTSheetSpecsInProgressByUserAsync(this.CurrentUserService.UserId ?? "ANONYMOUS", tSheetSpecsInProgressDto);
            return Created(nameof(this.SaveTSheetSpecsInProgressByUser), userId);
        }


        [HttpDelete("DeleteTSheetSpecsInProgressBySerial/{productSerial}")]
        [AllowAnonymous]
        public string DeleteTSheetSpecsInProgressBySerial(string productSerial)
        {
            var deletedProductSerial = this.TSheetSpecsInProgressService.DeleteTSheetSpecsInProgressByProductSerial(productSerial);
            return deletedProductSerial;
        }
    }
}
