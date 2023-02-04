using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Fims.Services.TReports;
using Fims.Services.TSheets;
using Fims.Data.Models.TReports;

using Fims.Data.Entities;
using Fims.Web.Server.Infrastructure.Services;
using Fims.Web.Server.Infrastructure.Extensions;
using static Fims.Common.Constants;
using System.IO;


namespace Fims.Web.Server.Controllers
{
    //[Authorize]
    //[Authorize(Roles = AdministratorRole)]
    [ApiController]
    [Route("api/[controller]")]
    public class TReportsController : ControllerBase
    {
        private readonly ITSheetsService tSheetsService;
        private readonly ITReportsService tReportsService;
        private readonly ICurrentUserService currentUserService;

        public TReportsController(
            ITSheetsService tSheetsService,
            ITReportsService tReportsService,
            ICurrentUserService currentUserService)
        {
            this.tSheetsService = tSheetsService;
            this.tReportsService = tReportsService;
            this.currentUserService = currentUserService;
        }

        /*
        [HttpGet("FindTSheetWithTItems/{id}")]
        [AllowAnonymous] //JBH
        public async Task<ActionResult<TSheet>> FindTSheetWithTItems(int id)
            => await this.tSheetsService.FindTSheetWithTItemsByIdAsync(id);

        // [HttpGet]
        // [AllowAnonymous]
        // public async Task<TSheetsComplexSearchResponseModel> Search(
        //     [FromQuery] TSheetsComplexSearchRequestModel searchRequest)
        //     => await this.tSheetsService.ComplexSearchAsync(searchRequest);
        */

        [HttpGet(nameof(AllTReportSpecs))]
        [AllowAnonymous]
        public async Task<List<string>> AllTReportSpecs()
        {
            var data = await this.tReportsService.AllTReportSpecsAsync();
            return data;
        }


        [HttpPost(nameof(GenerateTReport))]
        public async Task<ActionResult> GenerateTReport(TReportDto tReportRequest)
        {
            Stream tReportStream = await this.tReportsService.GenerateTReportAsync(tReportRequest);
            return File(tReportStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", tReportRequest.TReportOutputFile);
            //return Created(nameof(this.GenerateTReport), tReportStream);
        }

        /*
        [HttpPut(nameof(UpdateTSheet))]
        //[HttpPut(Id)]
        public async Task<ActionResult> UpdateTSheet(int id, TSheet tSheet)
            => await this.tSheetsService
                .UpdateAsync(id, tSheet, this.currentUserService.UserId)
                .ToActionResult();

        [HttpDelete("DeleteTSheet/{id}")]
        public async Task<ActionResult> DeleteTSheet(int id)
            => await this.tSheetsService
                .DeleteAsync(id)
                .ToActionResult();
        */
    }
}
