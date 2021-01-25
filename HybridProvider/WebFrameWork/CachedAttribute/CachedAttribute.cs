using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HybridProvider.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace HybridProvider.WebFrameWork.CachedAttribute
{
    //[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CachedAttribute : ActionFilterAttribute
    {
        private readonly int _secondsToCache;
        private readonly bool _useDefaultCacheSeconds;

        public CachedAttribute()
        {
            _useDefaultCacheSeconds = true;
        }
        public CachedAttribute(int secondsToCache)
        {
            _secondsToCache = secondsToCache;
            _useDefaultCacheSeconds = false;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();

            // Check if request has Cache
            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
            var cachedResponse = await cacheService.GetCachedResponseAsync(cacheKey);

            // If Yes => return Value
            if (!string.IsNullOrWhiteSpace(cachedResponse))
            {
                var contentResult = new ContentResult
                {
                    Content = cachedResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };
                context.Result = contentResult;
                return;
            }

            // If No => Go to method => Cache Value
            var actionExecutedContext = await next();

            if (actionExecutedContext.Result is OkObjectResult okObjectResult)
            {
                var secondsToCache = _useDefaultCacheSeconds ? 5 : _secondsToCache;
                await cacheService.CacheResponseAsync(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(secondsToCache));
            }

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
