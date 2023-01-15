using System.ComponentModel.DataAnnotations;

using static Fims.Data.Models.ErrorMessages;
using static Fims.Data.ModelConstants.Common;
using static Fims.Data.ModelConstants.Identity;


namespace Fims.Data.Models.Identity
{
    public class UserAuthInfoModel
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }

        public string HangulName { get; set; }
        public string EnglishName { get; set; }
        public string Email { get; set; }
    }
}
