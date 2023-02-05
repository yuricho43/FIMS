using System.ComponentModel.DataAnnotations;

using static Fims.Data.Models.ErrorMessages;
using static Fims.Data.ModelConstants.Common;
using static Fims.Data.ModelConstants.Identity;


namespace Fims.Data.Models.Identity
{
    public class RegisterRequestModel
    {
        [Required]
        [StringLength(MaxUserNameLength, MinimumLength = MinUserNameLength, ErrorMessage = IdLengthErrorMessage)]
        public string UserName { get; set; } //login name

        [Required]
        [MinLength(MinPasswordLength, ErrorMessage = PasswordLengthErrorMessage)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [StringLength(MaxRoleNameLength, MinimumLength = MinRoleNameLength, ErrorMessage = StringLengthErrorMessage)]
        public string Role { get; set; }

        [Required]
        [EmailAddress(ErrorMessage= EmailFormatErrorMessage)]
        //[MinLength(MinEmailLength)]
        //[MaxLength(MaxEmailLength)]
        public string Email { get; set; }

        [Required]
        [StringLength(MaxNameLength, MinimumLength = MinNameLength, ErrorMessage = NameLengthErrorMessage)]
        public string HangulName { get; set; }

        ////[Required]
        //[StringLength(MaxNameLength,
        //              ErrorMessage = StringLengthErrorMessage,
        //              MinimumLength = MinNameLength)]
        //public string EnglishName { get; set; }

        ////[Required]
        //[MinLength(MinPasswordLength)]
        //[DataType(DataType.Password)]
        //[Compare(nameof(Password), ErrorMessage = PasswordsDoNotMatchErrorMessage)]
        //public string ConfirmPassword { get; set; }
    }
}
