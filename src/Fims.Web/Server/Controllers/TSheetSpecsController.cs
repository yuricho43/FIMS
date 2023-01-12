using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        public TSheetSpecsController(
            ITSheetSpecsService tSheetSpecsService,
            ICurrentUserService currentUserService)
        {
            this.TSheetSpecsService = tSheetSpecsService;
            this.CurrentUserService = currentUserService;
        }

        // GET: api/TSheetSpecs/TSheetModels
        [HttpGet("TSheetModels")]
        [AllowAnonymous] //JBH
        public async Task<List<string>> TSheetModels()
        {
            var data = await this.TSheetSpecsService.GetEquipmentModelsAsync();
            return data;
        }

        // GET: api/TSheetSpecs/TSheetSpecsDict
        [HttpGet("TSheetSpecsDict")]
        [AllowAnonymous] //JBH
        public async Task<Dictionary<string, TSheetSpec>> GetTSheetSpecsDictAsync()
        {
            var data = await this.TSheetSpecsService.GetTSheetSpecsDictAsync();
            return data;
        }

        // GET: api/TSheetSpecs/TSheetSpecByModel/{equipmentModel}
        [HttpGet("TSheetSpecByModel/{equipmentModel}")]
        [AllowAnonymous] //JBH
        public async Task<TSheetSpec> TSheetSpecByModel(string equipmentModel)
        {
            var data = await this.TSheetSpecsService.GetTSheetSpecByEquipmentModelAsync(equipmentModel);
            return data;
        }
    }
}
