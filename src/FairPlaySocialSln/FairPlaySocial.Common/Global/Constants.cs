namespace FairPlaySocial.Common.Global
{
    public static class Constants
    {
        public static class Policies
        {
            public static class RateLimiting
            {
                public const string HomeFeed = nameof(HomeFeed);
            }
        }
        public static class GeoCoordinates
        {
            /// <summary>
            /// 4326 refers to WGS 84, a standard used in GPS and other geographic systems.
            /// Check: https://learn.microsoft.com/en-us/ef/core/modeling/spatial
            /// </summary>
            public const int SRID = 4326;
        }
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
            public static class ClientSideErrorLogController
            {
                public const string AddClientSideError = "api/ClientSideErrorLog/AddClientSideError";
            }
            public static class VisitorTrackingController
            {
                public const string TrackAnonymousClientInformation = "api/VisitorTracking/TrackAnonymousClientInformation";
                public const string TrackAuthenticatedClientInformation = "api/VisitorTracking/TrackAuthenticatedClientInformation";
                public const string UpdateVisitTimeElapsed = "api/VisitorTracking/UpdateVisitTimeElapsed";
            }
            public static class LocalizationController
            {
                public const string GetAllResources = "api/Localization/GetAllResources";
                public const string GetSupportedCultures = "api/Localization/GetSupportedCultures";
            }
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
                public const string GroupFeed = $"/User/Feeds/{nameof(GroupFeed)}";
                public const string UpdateMyUserPreferences = $"/User/{nameof(UpdateMyUserPreferences)}";
                public const string CreateMyPost = $"/User/Posts/{nameof(CreateMyPost)}";
                public const string UpdateMyUserProfile = $"/User/UserProfile/{nameof(UpdateMyUserProfile)}";
                public const string Post = $"/User/Posts/{nameof(Post)}";
                public const string CreateMyGroup = $"/User/Groups/{nameof(CreateMyGroup)}";
                public const string SearchUserProfiles = $"/User/Search/{nameof(SearchUserProfiles)}";
                public const string SearchPosts = $"/User/Search/{nameof(SearchPosts)}";
                public const string SearchGroups = $"/User/Search/{nameof(SearchGroups)}]";
            }

            public static class PublicPagesRoutes
            {
                public const string EmbeddedPost = $"/Public/{nameof(EmbeddedPost)}";
                public const string UserFeed = $"/Public/UserFeed";
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
