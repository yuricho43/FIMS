using System.ComponentModel.DataAnnotations;

using static Fims.Data.ModelConstants.Identity;


namespace Fims.Data.Models.Identity
{
    public class LoginRequestModel
    {
        [Required]
        [MinLength(MinUserNameLength)]
        [MaxLength(MaxUserNameLength)]
        public string UserName { get; set; } //login name

        // [Required]
        // [EmailAddress]
        // [MinLength(MinEmailLength)]
        // [MaxLength(MaxEmailLength)]
        // public string Email { get; set; }

        [Required]
        [MinLength(MinPasswordLength)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
