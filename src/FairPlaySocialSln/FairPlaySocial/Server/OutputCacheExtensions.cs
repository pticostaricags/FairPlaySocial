namespace FairPlaySocial.Server
{
    public static class OutputCacheExtensions
    {
        public static IServiceCollection ConfigurePlatformOutputCache(this IServiceCollection services)
        {
            services.AddOutputCache(configureOptions =>
            {
                configureOptions.AddPolicy(Common.Global.Constants.Policies.OutputCaching.LocalizationResources,
                    policy =>
                    {

                        policy.SetVaryByHeader("accept-language");
                        policy.Expire(TimeSpan.FromMinutes(1));
                    });
                configureOptions.AddPolicy(Common.Global.Constants.Policies.OutputCaching.SupportedCultures,
                    policy =>
                    {
                        policy.Expire(TimeSpan.FromMinutes(1));
                    });
            });
            return services;
        }
    }
}
