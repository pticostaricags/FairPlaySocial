namespace FairPlaySocial.Common.Global
{
    public static class Constants
    {
        public static class Pagination
        {
            public const int DefaultPageSize = 5;
        }
        public static class Locales
        {
            public const string DefaultLocale = "en-US";
        }
        public static class Claims
        {
            public const string ApplicationUserId = nameof(ApplicationUserId);
            public const string ObjectIdentifier = "http://schemas.microsoft.com/identity/claims/objectidentifier";
            public const string Name = "name";
            public const string GivenName = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname";
            public const string SurName = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname";
            public const string Emails = "emails";
        }

        public static class Assemblies
        {
            public const string MainAppAssemblyName = "FairPlaySocial";
        }

        public static class ApiRoutes
        {
            public static class AuthController
            {
                public const string GetMyRoles = "api/Auth/GetMyRoles";
            }
        }
        public static class Hubs
        {
            public const string HomeFeedHub = $"/{nameof(HomeFeedHub)}";
            public const string ReceiveMessage = "ReceiveMessage";
            public const string SendMessage = "SendMessage";
        }

        public static class Roles
        {
            public const string Admin = nameof(Admin);
            public const string User = nameof(User);
        }

        public static class MauiBlazorAppPages
        {
            public static class AdminRolePagesRoutes
            {
                public const string UserList = $"/Admin/{nameof(UserList)}";
            }

            public static class UserRolePagesRoutes
            {
                public const string HomeFeed = $"/User/Feeds/{nameof(HomeFeed)}";
                public const string UpdateMyUserPreferences = $"/User/{nameof(UpdateMyUserPreferences)}";
                public const string CreateMyPost = $"/User/Posts/{nameof(CreateMyPost)}";
                public const string UpdateMyUserProfile = $"/User/UserProfile/{nameof(UpdateMyUserProfile)}";
            }
        }

        public static class EntityNames
        {
            public const string ApplicationUser = nameof(ApplicationUser);
            public const string Photo = nameof(Photo);
            public const string UserProfile = nameof(UserProfile);
        }
    }
}
