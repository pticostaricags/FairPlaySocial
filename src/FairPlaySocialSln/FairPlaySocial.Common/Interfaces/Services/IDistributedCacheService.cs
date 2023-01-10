using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Common.Interfaces.Services
{
    public interface IDistributedCacheService
    {
        Task SetStringValueAsync(string key, string value, CancellationToken cancellationToken);
        Task<string?> GetStringValueAsync(string key, CancellationToken cancellationToken);
    }
}
