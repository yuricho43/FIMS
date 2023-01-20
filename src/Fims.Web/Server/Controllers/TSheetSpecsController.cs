using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http.Headers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

using Fims.Data.Models.TSheetSpecs;
using Fims.Services.TSheetSpecs;

using Fims.Web.Server.Infrastructure.Services;
using System.Linq;

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
        [HttpGet("TSheetModels")]
        [AllowAnonymous]
        public async Task<List<string>> TSheetModels()
        {
            var data = await this.TSheetSpecsService.GetEquipmentModelsAsync();
            return data;
        }

        // GET: api/TSheetSpecs/TSheetSpecsDict
        [HttpGet("TSheetSpecsDict")]
        [AllowAnonymous]
        public async Task<Dictionary<string, TSheetSpec>> GetTSheetSpecsDictAsync()
        {
            var data = await this.TSheetSpecsService.GetTSheetSpecsDictAsync();
            return data;
        }

        // GET: api/TSheetSpecs/TSheetSpecByModel/{equipmentModel}
        [HttpGet("TSheetSpecByModel/{equipmentModel}")]
        [AllowAnonymous]
        public async Task<TSheetSpec> TSheetSpecByModel(string equipmentModel)
        {
            var data = await this.TSheetSpecsService.GetTSheetSpecByEquipmentModelAsync(equipmentModel);
            return data;
        }


        [HttpPost(nameof(Save))]
        [AllowAnonymous]
        public async Task<bool> Save(IEnumerable<IFormFile> files)
        {
            var specFormFile = files.First();
            var result = await this.TSheetSpecsService.SaveAsync(specFormFile);
            return result;
        }

        [HttpPost(nameof(Remove))]
        [AllowAnonymous]
        public async Task<bool> Remove(string[] files)
        {
            var result = await this.TSheetSpecsService.RemoveAsync(files);
            return result;
        }
    }
}
