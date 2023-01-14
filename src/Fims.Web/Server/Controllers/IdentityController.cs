using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Fims.Data.Models.Identity;
using Fims.Services.Identity;
using Fims.Web.Server.Infrastructure.Services;
using Fims.Web.Server.Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;
using Fims.Data.Entities;

namespace Fims.Web.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService identityService;
        private readonly ICurrentUserService currentUserService;

        public IdentityController(
            IIdentityService identityService, 
            ICurrentUserService currentUserService)
        {
            this.identityService = identityService;
            this.currentUserService = currentUserService;
        }

        [HttpPost(nameof(Register))]
        public async Task<ActionResult> Register(
            RegisterRequestModel model)
            => await this.identityService
                .RegisterAsync(model)
                .ToActionResult();

        [HttpPost(nameof(Login))]
        public async Task<ActionResult<LoginResponseModel>> Login(
            LoginRequestModel model)
            => await this.identityService
                .LoginAsync(model)
                .ToActionResult();

        [Authorize]
        [HttpPut(nameof(ChangeUserProfile))]
        public async Task<ActionResult> ChangeUserProfile(
            ChangeUserProfileRequestModel model)
            => await this.identityService
                .ChangeUserProfileAsync(model, this.currentUserService.UserId)
                .ToActionResult();

        [Authorize]
        [HttpPut(nameof(ChangePassword))]
        public async Task<ActionResult> ChangePassword(
            ChangePasswordRequestModel model)
            => await this.identityService
                .ChangePasswordAsync(model, this.currentUserService.UserId)
                .ToActionResult();

        // GET: api/Identity/GetAllUsers
        [HttpGet("GetAllUsers")]
        [AllowAnonymous]
        public async Task<List<UserAuthInfoModel>> GetAllUsers()
        {
            var data = await this.identityService.AllUsers();
            return data;
        }

        // GET: api/Identity/GetRoles
        [HttpGet("GetRoles")]
        [AllowAnonymous]
        public async Task<List<FimsRole>> GetRoles()
        {
            var data = await this.identityService.Roles();
            return data;
        }

    }
}
