namespace FairPlaySocial.Server
{
    /// <summary>
    /// Output cache extensions.
    /// </summary>
    public static class OutputCacheExtensions
    {
        /// <summary>
        /// Configures output cache.
        /// </summary>
        /// <param name="services">Services to configure.</param>
        /// <returns><see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection ConfigurePlatformOutputCache(this IServiceCollection services)
        {
            services.AddOutputCache(configureOptions =>
            {
                configureOptions.AddPolicy(Common.Global.Constants.Policies.OutputCaching.LocalizationResources,
                    policy =>
                    {
                        policy.SetVaryByHeader(Microsoft.Net.Http.Headers.HeaderNames.AcceptLanguage);
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
