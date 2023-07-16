namespace Fims.Web.Server.Infrastructure.Services
{
    public interface ICurrentUserService
    {
        string UserId { get; }      // UserId here is the db index for the user.
        string UserName { get; }    // "coolbix"
    }
}
