using FairPlaySocial.Common.Interfaces.Services;
using Microsoft.Extensions.Caching.Distributed;
using System.Drawing;

namespace FairPlaySocial.Server.Services
{
    public class SqlServerDistributedCacheService : IDistributedCacheService
    {
        private readonly IDistributedCache sqlServerDisitributedCache;
        private ILogger<SqlServerDistributedCacheService> logger;

        public SqlServerDistributedCacheService(IDistributedCache sqlServerDisitributedCache,
            ILogger<SqlServerDistributedCacheService> logger)
        {
            this.sqlServerDisitributedCache = sqlServerDisitributedCache;
            this.logger = logger;
        }

        public async Task SetStringValueAsync(string key, string value, CancellationToken cancellationToken)
        {
            this.logger.LogInformation(message: $"Setting value for {nameof(key)} = {key}");
            await this.sqlServerDisitributedCache.SetStringAsync(key: key, value: value);
        }

        public async Task<string?> GetStringValueAsync(string key, CancellationToken cancellationToken)
        {
            this.logger.LogInformation(message: $"Reading value for {nameof(key)} = {key}");
            var result = await this.sqlServerDisitributedCache.GetStringAsync(key: key, token: cancellationToken);
            return result;
        }
    }
}
