using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyCaching.Core;
using InMemoryProvider.Services;

namespace InMemoryProvider.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ICacheService _cacheService;

        public ValuesController(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        [HttpGet("GetTime")]
        public async Task<IActionResult> GetTime()
        {
            var cacheKey = GenerateCacheKeyFromRequest(HttpContext.Request);

            var cachedResponse = await _cacheService.GetCachedResponseAsync(cacheKey);
            if (!string.IsNullOrWhiteSpace(cachedResponse))
                return Ok(cachedResponse);

            var data = DateTime.Now.Second;
            await _cacheService.CacheResponseAsync(cacheKey, data, TimeSpan.FromSeconds(5));
            return Ok(data);
        }

        // Generate Cache Key
        private static string GenerateCacheKeyFromRequest(HttpRequest httpRequest)
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append($"{httpRequest.Path}");
            foreach (var (key, value) in httpRequest.Query.OrderBy(x => x.Key))
            {
                if (!key.Equals("refreshCache"))
                    keyBuilder.Append($"|{key}-{value}");
            }

            return keyBuilder.ToString();
        }

    }
}
