using System.ComponentModel.DataAnnotations;

using static Fims.Data.Models.ErrorMessages;
using static Fims.Data.ModelConstants.Common;
using static Fims.Data.ModelConstants.Identity;


namespace Fims.Data.Models.Identity
{
    public class RegisterRequestModel
    {
        [Required]
        [EmailAddress]
        [MinLength(MinEmailLength)]
        [MaxLength(MaxEmailLength)]
        public string Email { get; set; }

        [Required]
        [MinLength(MinPasswordLength)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [StringLength(
            MaxNameLength,
            ErrorMessage = StringLengthErrorMessage,
            MinimumLength = MinNameLength)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(
            MaxNameLength,
            ErrorMessage = StringLengthErrorMessage,
            MinimumLength = MinNameLength)]
        public string LastName { get; set; }

        [Required]
        [MinLength(MinPasswordLength)]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = PasswordsDoNotMatchErrorMessage)]
        public string ConfirmPassword { get; set; }
    }
}
