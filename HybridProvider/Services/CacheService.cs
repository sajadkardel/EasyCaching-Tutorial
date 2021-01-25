using System;
using System.Threading.Tasks;
using EasyCaching.Core;
using Newtonsoft.Json;

namespace HybridProvider.Services
{
    public class CacheService : ICacheService
    {
        private readonly IHybridCachingProvider _hybridCachingProvider;
        public CacheService(IHybridCachingProvider hybridCachingProvider)
        {
            _hybridCachingProvider = hybridCachingProvider;
        }

        public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
        {
            if (response == null) return;
            var serializedResponse = JsonConvert.SerializeObject(response);
            await _hybridCachingProvider.SetAsync(cacheKey, serializedResponse, timeToLive);
        }

        public async Task<string> GetCachedResponseAsync(string cacheKey)
        {
            var cachedResponse = await _hybridCachingProvider.GetAsync<string>(cacheKey);
            return string.IsNullOrWhiteSpace(cachedResponse.Value) ? null : cachedResponse.Value;
        }
    }
}
