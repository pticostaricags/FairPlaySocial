using FairPlaySocial.ClientServices;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.Models.UserPreference;
using FairPlaySocial.MultiplatformServices;
using Microsoft.Extensions.DependencyInjection;

namespace FairPlaySocial.ClientsConfiguration
{
    public static class ClientsInitialization
    {
        public static void AddMultiPlatformServices(this IServiceCollection services)
        {
            #region START OF TRANSIENT SERVICES
            services.AddTransient<INavigationService, NavigationService>();
            services.AddTransient<HttpClientService>();
            services.AddTransient<ApplicationUserClientService>();
            services.AddTransient<PhotoClientService>();
            services.AddTransient<MyUserPreferenceClientService>();
            services.AddTransient<MyPostClientService>();
            services.AddTransient<MyFeedClientService>();
            services.AddTransient<MyFollowClientService>();
            services.AddTransient<MyUserProfileClientService>();
            services.AddTransient<PublicUserProfileClientService>();
            services.AddTransient<MyLikedPostsClientService>();
            services.AddTransient<PublicFeedClientService>();
            services.AddTransient<PostCommentClientService>();
            services.AddTransient<VisitorTrackingClientService>();
            services.AddTransient<ClientSideErrorLogClientService>();
            services.AddTransient<MyGroupClientService>();
            services.AddTransient<SearchClientService>();
            services.AddTransient<UserMessageClientService>();
            services.AddTransient<ProfileVisitorClientService>();
            #endregion END OF TRANSIENT SERVICES

            #region START OF SINGLETON SERVICES
            services.AddSingleton<LocalizationClientService>();
            services.AddSingleton(new UserPreferenceModel());
            services.AddSingleton<IWhiteLabelingService, WhiteLabelingService>();
            #endregion END OF SINGLETON SERVICES
        }
    }
}