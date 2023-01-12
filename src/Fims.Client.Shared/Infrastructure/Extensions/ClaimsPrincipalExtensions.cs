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
    }
}