using System;
using System.Threading.Tasks;
using EasyCaching.Core;
using Newtonsoft.Json;

namespace RedisProvider.Services
{
    public class CacheService : ICacheService
    {
        private readonly IEasyCachingProvider _easyCachingProvider;
        public CacheService(IEasyCachingProvider easyCachingProvider)
        {
            _easyCachingProvider = easyCachingProvider;
        }

        public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
        {
            if (response == null) return;
            var serializedResponse = JsonConvert.SerializeObject(response);
            await _easyCachingProvider.SetAsync(cacheKey, serializedResponse, timeToLive);
        }

        public async Task<string> GetCachedResponseAsync(string cacheKey)
        {
            var cachedResponse = await _easyCachingProvider.GetAsync<string>(cacheKey);
            return string.IsNullOrWhiteSpace(cachedResponse.Value) ? null : cachedResponse.Value;
        }
    }
}
