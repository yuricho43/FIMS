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
        public async Task<ActionResult> GetTSheetSpecsInProgressByUser(string userId)
        {
            var data = await this.TSheetSpecsInProgressService.GetTSheetSpecsInProgressAsync(this.CurrentUserService.UserId ?? "ANONYMOUS");
            return Created(nameof(this.GetTSheetSpecsInProgressByUser), data);
        }

        [HttpPost(nameof(SaveTSheetSpecsInProgressByUser))]
        [AllowAnonymous] //JBH
        public async Task<ActionResult> SaveTSheetSpecsInProgressByUser(TSheetSpecsInProgressReqeust tSheetSpecsInProgressReqeust)
        {
            var userId = tSheetSpecsInProgressReqeust.UserId;
            var tSheetSpecsJsonString = tSheetSpecsInProgressReqeust.TSheetSpecsJsonString;

            var fileName = await this.TSheetSpecsInProgressService.SaveTSheetSpecsInProgressByUserAsync(tSheetSpecsJsonString, this.CurrentUserService.UserId ?? "ANONYMOUS");
            return Created(nameof(this.SaveTSheetSpecsInProgressByUser), fileName);
        }
    }
}
