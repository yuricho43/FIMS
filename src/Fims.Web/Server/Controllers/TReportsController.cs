using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Fims.Services.TReports;
using Fims.Services.TSheets;
using Fims.Data.Models.TReports;

using Fims.Data.Entities;
using Fims.Web.Server.Infrastructure.Services;
using Fims.Web.Server.Infrastructure.Extensions;
using static Fims.Common.Constants;

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

        [Authorize]
        [HttpGet(nameof(AllTReportSpecs))]
        public List<string> AllTReportSpecs()
        {
            var data = this.tReportsService.AllTReportSpecs();
            return data;
        }


        [Authorize]
        [HttpPost(nameof(GenerateTReport))]
        public async Task<ActionResult> GenerateTReport(TReportDto tReportRequest)
        {
            Stream tReportStream = await this.tReportsService.GenerateTReportAsync(tReportRequest);
            return File(tReportStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", tReportRequest.TReportOutputFile);
            //return Created(nameof(this.GenerateTReport), tReportStream);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost(nameof(UploadSpecFile))]
        public async Task<string> UploadSpecFile(IEnumerable<IFormFile> files)
        {
            var specFormFile = files.First();
            var result = await this.tReportsService.UploadSpecFileAsync(specFormFile);
            return result;
        }
    }
}
