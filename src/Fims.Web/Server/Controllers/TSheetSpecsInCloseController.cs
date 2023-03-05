using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        public TSheetSpecsInCloseController(
            ITSheetSpecsInCloseService tSheetSpecsSaveService,
            ICurrentUserService currentUserService)
        {
            this.TSheetSpecsInCloseService = tSheetSpecsSaveService;
            this.CurrentUserService = currentUserService;
        }


        [Authorize]
        [HttpGet("{userId}")]
        public async Task<ActionResult> GetTSheetSpecsInCloseByUser(string userId)
        {
            // "userId" should be same with "this.CurrentUserService.UserId", and unused now.
            var tSheetSpecsInCloseDto = await this.TSheetSpecsInCloseService.GetTSheetSpecsInCloseAsync(this.CurrentUserService.UserId ?? "ANONYMOUS");
            return Created(nameof(this.GetTSheetSpecsInCloseByUser), tSheetSpecsInCloseDto);
        }


        [Authorize]
        [HttpPost(nameof(SaveTSheetSpecsInCloseByUser))]
        public async Task<ActionResult> SaveTSheetSpecsInCloseByUser(TSheetSpecsInProgressDto tSheetSpecsInCloseDto)
        {
            // "userId" should be same with "this.CurrentUserService.UserId", and unused now.
            var userId = tSheetSpecsInCloseDto.UserId;
            var fileName = await this.TSheetSpecsInCloseService.SaveTSheetSpecsInCloseAsync(this.CurrentUserService.UserId ?? "ANONYMOUS", tSheetSpecsInCloseDto);
            return Created(nameof(this.SaveTSheetSpecsInCloseByUser), userId);
        }


        [Authorize]
        [HttpDelete("DeleteTSheetSpecsInCloseBySerial/{productSerial}")]
        public string DeleteTSheetSpecsInCloseBySerial(string productSerial)
        {
            var deletedProductSerial = this.TSheetSpecsInCloseService.DeleteTSheetSpecsInCloseByProductSerial(productSerial);
            return deletedProductSerial;
        }
    }
}
