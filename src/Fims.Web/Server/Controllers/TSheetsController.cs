using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Fims.Services.TSheetSpecs;
using Fims.Services.TSheets;
using Fims.Data.Models.TSheets;

using Fims.Data.Entities;
using Fims.Web.Server.Infrastructure.Services;
using Fims.Web.Server.Infrastructure.Extensions;
using static Fims.Common.Constants;


namespace Fims.Web.Server.Controllers
{
    //[Authorize]
    //[Authorize(Roles = AdministratorRole)]
    public class TSheetsController : ApiController
    {
        private readonly ITSheetsService     tSheetsService;
        private readonly ITSheetSpecsService tSheetSpecsService;        
        private readonly ICurrentUserService currentUserService;

        public TSheetsController(
            ITSheetsService tSheetsService,
            ITSheetSpecsService tSheetSpecsService,
            ICurrentUserService currentUserService)
        {
            this.tSheetsService = tSheetsService;
            this.tSheetSpecsService = tSheetSpecsService;
            this.currentUserService = currentUserService;
        }

        [HttpGet]
        //[HttpGet(Name = "TSheets")] //JBH
        [AllowAnonymous] //JBH
        public async Task<IEnumerable<TSheet>> All()
        {
            var data = await this.tSheetsService.AllTSheetsAsync();
            return data;
        }

        [HttpGet(Id)]
        [AllowAnonymous] //JBH
        public async Task<ActionResult<TSheet>> FindTSheetWithTItems(int id)
            => await this.tSheetsService.FindTSheetWithTItemsByIdAsync(id);

        // [HttpGet]
        // [AllowAnonymous]
        // public async Task<TSheetsComplexSearchResponseModel> Search(
        //     [FromQuery] TSheetsComplexSearchRequestModel searchRequest)
        //     => await this.tSheetsService.ComplexSearchAsync(searchRequest);

        [HttpPost(nameof(AddTSheet))]
        //[HttpPost]
        public async Task<ActionResult> AddTSheet(TSheet tSheet)
        {
            var id = await this.tSheetsService.CreateAsync(tSheet, this.currentUserService.UserId);
            return Created(nameof(this.AddTSheet), id);
        }

        [HttpPut(nameof(UpdateTSheet))]
        //[HttpPut(Id)]
        public async Task<ActionResult> UpdateTSheet(int id, TSheet tSheet)
            => await this.tSheetsService
                .UpdateAsync(id, tSheet, this.currentUserService.UserId)
                .ToActionResult();

        [HttpDelete(nameof(DeleteTSheet) + PathSeparator + Id)]
        //[HttpDelete(Id)]
        public async Task<ActionResult> DeleteTSheet(int id)
            => await this.tSheetsService
                .DeleteAsync(id)
                .ToActionResult();
    }
}
