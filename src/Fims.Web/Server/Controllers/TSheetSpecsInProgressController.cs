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
    public class TSheetSpecsInProgressController : ApiController
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
        [AllowAnonymous] //JBH
        public async Task<ActionResult> GetTSheetSpecsInProgressByUser(string userIdRx)
        {
            // "userIdRx" should be same with "this.CurrentUserService.UserId", and unused now.
            var userId = this.CurrentUserService.UserId ?? "ANONYMOUS";
            var tSheetSpecsInProgressDto = await this.TSheetSpecsInProgressService.GetTSheetSpecsInProgressAsync(userId);
            return Created(nameof(this.GetTSheetSpecsInProgressByUser), tSheetSpecsInProgressDto);
        }


        [HttpPost(nameof(SaveTSheetSpecsInProgressByUser))]
        [AllowAnonymous] //JBH
        public async Task<ActionResult> SaveTSheetSpecsInProgressByUser(TSheetSpecsInProgressDto tSheetSpecsInProgressDto)
        {
            // "userIdRx" should be same with "this.CurrentUserService.UserId", and unused now.
            var userIdRx = tSheetSpecsInProgressDto.UserId;
            var userId = this.CurrentUserService.UserId ?? "ANONYMOUS";
            var fileName = await this.TSheetSpecsInProgressService.SaveTSheetSpecsInProgressByUserAsync(userId, tSheetSpecsInProgressDto);
            return Created(nameof(this.SaveTSheetSpecsInProgressByUser), userId);
        }
    }
}
