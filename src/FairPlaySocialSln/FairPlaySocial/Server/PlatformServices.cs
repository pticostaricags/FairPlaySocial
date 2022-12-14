using FairPlaySocial.Services;
using Microsoft.Extensions.Configuration;
using PTI.Microservices.Library.Configuration;
using PTI.Microservices.Library.IpData.Configuration;
using PTI.Microservices.Library.IpData.Services;
using PTI.Microservices.Library.Services;

namespace FairPlaySocial.Server
{
    public static class PlatformServices
    {
        internal static void AddPlatformServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddTransient<ApplicationUserService>();
            services.AddTransient<PhotoService>();
            services.AddTransient<UserPreferenceService>();
            services.AddTransient<PostService>();
            services.AddTransient<ApplicationUserFollowService>();
            services.AddTransient<UserProfileService>();
            services.AddTransient<LikedPostService>();
            services.AddTransient<DislikedPostService>();
            services.AddTransient<PostTagService>();
            services.AddTransient<PostUrlService>();
            services.AddTransient<ForbiddenUrlService>();
            services.AddTransient<VisitorTrackingService>();
            services.AddTransient<ClientSideErrorLogService>();
            services.AddTransient<GroupService>();
            services.AddTransient<SearchService>();
            services.AddTransient<GroupMemberService>();
            services.AddTransient<ErrorLogService>();
            services.AddTransient<PostKeyPhraseService>();
            services.ConfigureIpDataService(configuration);
            services.ConfigureIpStackService(configuration);
        }

        private static void ConfigureIpDataService(this IServiceCollection services, 
            IConfiguration configuration)
        {
            IpDataConfiguration ipDataConfiguration =
                            configuration.GetSection(nameof(IpDataConfiguration))
                            .Get<IpDataConfiguration>()!;
            services.AddSingleton(ipDataConfiguration);
            services.AddTransient<IpDataService>();
        }

        private static void ConfigureIpStackService(this IServiceCollection services,
            IConfiguration configuration)
        {
            IpStackConfiguration ipStackConfiguration =
                            configuration.GetSection(nameof(IpStackConfiguration))
                            .Get<IpStackConfiguration>()!;
            services.AddSingleton(ipStackConfiguration);
            services.AddTransient<IpStackService>();
        }

        public static void ConfigureAzureTextAnalyticsService(this IServiceCollection services,
            IConfiguration configuration)
        {
            AzureTextAnalyticsConfiguration azureTextAnalyticsConfiguration =
                configuration.GetSection(nameof(AzureTextAnalyticsConfiguration))
                .Get<AzureTextAnalyticsConfiguration>()!;
            services.AddSingleton(azureTextAnalyticsConfiguration);
            services.AddTransient<AzureTextAnalyticsService>();
        }
    }
}
