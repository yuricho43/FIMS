using System.ComponentModel.DataAnnotations;

using static Fims.Data.Models.ErrorMessages;
using static Fims.Data.ModelConstants.Common;
using static Fims.Data.ModelConstants.Identity;


namespace Fims.Data.Models.Identity
{
    public class UserAuthInfoModel
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Role { get; set; }
    }
}
