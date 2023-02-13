using System;
using System.Security.Claims;

using Microsoft.AspNetCore.Http;


namespace Fims.Web.Server.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            //JBH: For example, TSheetsController needs to know who is the current user.

            var user = httpContextAccessor.HttpContext?.User;
            //JBH: "ClaimsPrincipal" of System.Security.Claims.ClaimsPrincipal is the "User" in plain english.
            //     In the context of Authenticaion, "Principal" means "User".

            if (user == null)
            {
                throw new InvalidOperationException("This request does not have an authenticated user.");
            }

            this.UserId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            /*
             * user.Claims:	
             *     [0]	{http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier:  c8cd5a1a-077b-448f-817f-4c19b1fa70dc}
             *     [1]	{http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress:    coolbix@hanmail.net}
             *     [2]	{http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name:            Cool}
             *     [3]	{http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname:         Bix}
             */        
        }

        public string UserId { get; }
    }
}
