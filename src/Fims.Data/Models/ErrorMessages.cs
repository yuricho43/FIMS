namespace Fims.Data.Models
{
    internal class ErrorMessages
    {
        // public const string StringLengthErrorMessage 
        //     = "The {0} must be at least {2} and at max {1} characters long.";
        // 
        // public const string PasswordsDoNotMatchErrorMessage 
        //     = "The password and confirmation password do not match.";

        public const string IdLengthErrorMessage
            = "{0}: {2}자 ~ {1}자 영문.";

        public const string PasswordLengthErrorMessage
            = "{0}: {1}자이상 영문숫자.";

        public const string NameLengthErrorMessage
            = "{0}: {2}자 ~ {1}자 문자열.";

        public const string StringLengthErrorMessage
            = "{0}: {2}자 ~ {1}자 문자열.";

        public const string EmailFormatErrorMessage
            = "email 형식이 아닙니다.";

        public const string PasswordsDoNotMatchErrorMessage
            = "확인암호와 일치하지 않습니다.";

    }
}