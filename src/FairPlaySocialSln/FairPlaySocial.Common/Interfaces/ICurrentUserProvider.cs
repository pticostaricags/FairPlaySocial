namespace FairPlaySocial.Common.Interfaces
{
    public interface ICurrentUserProvider
    {
        string GetUsername();
        string GetObjectId();
        bool IsLoggedIn();
        long GetApplicationUserId();
    }
}
