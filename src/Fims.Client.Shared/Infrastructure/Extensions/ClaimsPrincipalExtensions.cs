using Fims.Common;
using System.Security.Claims;


namespace Fims.Client.Shared.Infrastructure.Extensions
{
    /*
     *  JWT Token consists of 3 parts separated by "."
     *  
     *      - Header (Algorithm & Token type)
     *  
     *      - Payload --> ClaimsPrincipal
     *          . ClaimTypes.NameIdentifier:        "f01b2252-3710-4e64-a45a-e285c9eee85f"  (this is the ID index in Db)
     *          . CustomClaimTypes.UserName:        "inspector1"
     *          . ClaimTypes.Email:                 "inspector1@fstc.co.kr"
     *          . CustomClaimTypes.HangulName:      "김철수"
     *          . CustomClaimTypes.EnglishName:     "KCS"
     *          . ClaimTypes.Role:                  "Inspector"
     *          
     *      - Signature
     */

    //JBH:  https://www.jerriepelser.com/blog/useful-claimsprincipal-extension-methods/

    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal claimsPrincipal)    // UserId here is the db index for the user.
            => claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

        public static string GetUserName(this ClaimsPrincipal claimsPrincipal)
            => claimsPrincipal.FindFirstValue(CustomClaimTypes.UserName);       //NOTE: using CustomClaimTypes

        public static string GetHangulName(this ClaimsPrincipal claimsPrincipal)
            => claimsPrincipal.FindFirstValue(CustomClaimTypes.HangulName);     //NOTE: using CustomClaimTypes

        public static string GetEnglishName(this ClaimsPrincipal claimsPrincipal)
            => claimsPrincipal.FindFirstValue(CustomClaimTypes.EnglishName);    //NOTE: using CustomClaimTypes

        public static string GetEmail(this ClaimsPrincipal claimsPrincipal)
            => claimsPrincipal.FindFirstValue(ClaimTypes.Email);

        public static string GetUserRole(this ClaimsPrincipal claimsPrincipal)
            => claimsPrincipal.FindFirstValue(ClaimTypes.Role);

        public static bool IsCurrentUser(this ClaimsPrincipal claimsPrincipal, string id)
        {
            var currentUserId = GetUserId(claimsPrincipal);
            return string.Equals(currentUserId, id, StringComparison.OrdinalIgnoreCase);
        }
    }
}