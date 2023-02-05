using System.ComponentModel.DataAnnotations;

using static Fims.Data.Models.ErrorMessages;
using static Fims.Data.ModelConstants.Common;


namespace Fims.Data.Models.Identity
{
    public class ChangeUserProfileRequestModel
    {
        [Required]
        [StringLength(MaxNameLength, MinimumLength = MinNameLength, ErrorMessage = NameLengthErrorMessage)]
        public string HangulName { get; set; }

        [Required]
        [StringLength(MaxNameLength, MinimumLength = MinNameLength, ErrorMessage = NameLengthErrorMessage)]
        public string EnglishName { get; set; }
    }
}