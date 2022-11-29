using FairPlaySocial.Services;

namespace FairPlaySocial.Server
{
    public static class PlatformServices
    {
        internal static void AddPlatformServices(this IServiceCollection services)
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
        }
    }
}
