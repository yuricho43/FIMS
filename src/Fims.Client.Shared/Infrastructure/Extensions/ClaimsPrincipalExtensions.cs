using System.Security.Claims;


namespace Fims.Client.Shared.Infrastructure.Extensions
{
    /*
     *  JWT Token consists of 3 parts separated by "."
     *  
     *      - Header (Algorithm & Token type)
     *  
     *      - Payload --> ClaimsPrincipal
     *          . ClaimTypes.NameIdentifier : "f01b2252-3710-4e64-a45a-e285c9eee85f"  (this is the ID index in Db)
     *          . ClaimTypes.Email:   "inspector@fstc.co.kr"
     *          . ClaimTypes.Name:    "김철수"
     *          . ClaimTypes.SurName: "KCS"
     *          . ClaimTypes.Role:    "Inspector"
     *  
     *      - Signature
     */

    //JBH:  https://www.jerriepelser.com/blog/useful-claimsprincipal-extension-methods/

    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal claimsPrincipal)
            => claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

        public static string GetEmail(this ClaimsPrincipal claimsPrincipal)
            => claimsPrincipal.FindFirstValue(ClaimTypes.Email);

        public static string GetFirstName(this ClaimsPrincipal claimsPrincipal)
            => claimsPrincipal.FindFirstValue(ClaimTypes.Name);

        public static string GetLastName(this ClaimsPrincipal claimsPrincipal)
            => claimsPrincipal.FindFirstValue(ClaimTypes.Surname);

        public static string GetUserRole(this ClaimsPrincipal claimsPrincipal)
          => claimsPrincipal.FindFirstValue(ClaimTypes.Role);

        public static string GetTenantId(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirstValue(CloudpressClaimTypes.TenantId);
        }

        public static bool IsCurrentUser(this ClaimsPrincipal claimsPrincipal, string id)
        {
            var currentUserId = GetUserId(claimsPrincipal);
            return string.Equals(currentUserId, id, StringComparison.OrdinalIgnoreCase);
        }

    }

    public static class CloudpressClaimTypes
    {
        public const string TenantId = "urn:cloudpress:tenant_id";

        // other custom claim types for my application...
    }
}