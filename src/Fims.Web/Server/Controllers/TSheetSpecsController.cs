using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Linq;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

using Fims.Data.Models.TSheetSpecs;
using Fims.Services.TSheetSpecs;
using Fims.Web.Server.Infrastructure.Services;

namespace Fims.Web.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TSheetSpecsController : ControllerBase
    {
        private readonly ITSheetSpecsService TSheetSpecsService;        
        private readonly ICurrentUserService CurrentUserService;
        private IWebHostEnvironment HostingEnvironment { get; set; }

        public TSheetSpecsController(
            ITSheetSpecsService tSheetSpecsService,
            ICurrentUserService currentUserService,
            IWebHostEnvironment hostingEnvironment)
        {
            this.TSheetSpecsService = tSheetSpecsService;
            this.CurrentUserService = currentUserService;
            this.HostingEnvironment = hostingEnvironment;
        }

        // GET: api/TSheetSpecs/TSheetModels
        [Authorize]
        [HttpGet("TSheetModels")]
        public async Task<List<string>> TSheetModels()
        {
            var data = await this.TSheetSpecsService.GetEquipmentModelsAsync();
            return data;
        }

        // GET: api/TSheetSpecs/TSheetSpecsDict
        [Authorize]
        [HttpGet("TSheetSpecsDict")]
        public async Task<Dictionary<string, TSheetSpec>> GetTSheetSpecsDictAsync()
        {
            var data = await this.TSheetSpecsService.GetTSheetSpecsDictAsync();
            return data;
        }

        // GET: api/TSheetSpecs/TSheetSpecByModel/{equipmentModel}
        [Authorize]
        [HttpGet("TSheetSpecByModel/{equipmentModel}")]
        public async Task<TSheetSpec> TSheetSpecByModel(string equipmentModel)
        {
            var data = await this.TSheetSpecsService.GetTSheetSpecByEquipmentModelAsync(equipmentModel);
            return data;
        }


        [Authorize(Roles = "Admin")]
        [HttpPost(nameof(UploadSpecFile))]
        public async Task<string> UploadSpecFile(IEnumerable<IFormFile> files)
        {
            var specFormFile = files.First();
            var result = await this.TSheetSpecsService.UploadSpecFileAsync(specFormFile);
            return result;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost(nameof(Remove))]
        public async Task<bool> Remove(string[] files)
        {
            var result = await this.TSheetSpecsService.RemoveAsync(files);
            return result;
        }
    }
}
