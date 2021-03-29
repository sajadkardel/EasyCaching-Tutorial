using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyCaching.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace RedisProvider.WebFrameWork.Middlewares
{
    public static class CacheMiddlewares
    {
        public static IApplicationBuilder UseWriteCaching(this IApplicationBuilder app)
        {
            return app.UseMiddleware<WriteCachingMiddleware>();
        }

        public static IApplicationBuilder UseReadCaching(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ReadCachingMiddleware>();
        }
    }

    public class WriteCachingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IEasyCachingProvider _easyCachingProvider;

        public WriteCachingMiddleware(RequestDelegate next, IEasyCachingProvider easyCachingProvider)
        {
            this._next = next;
            _easyCachingProvider = easyCachingProvider;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!await _easyCachingProvider.ExistsAsync(GenerateCacheKeyFromRequest(context.Request)))
            {
                await _easyCachingProvider.SetAsync(GenerateCacheKeyFromRequest(context.Request), 12, TimeSpan.FromMinutes(1));
            }
            await this._next(context);
        }

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

    public class ReadCachingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IEasyCachingProvider _easyCachingProvider;

        public ReadCachingMiddleware(RequestDelegate next, IEasyCachingProvider easyCachingProvider)
        {
            this._next = next;
            _easyCachingProvider = easyCachingProvider;
        }

        public async Task Invoke(HttpContext context)
        {
            var cacheValue = await _easyCachingProvider.GetAsync<object>(GenerateCacheKeyFromRequest(context.Request));
            await context.Response.WriteAsync(cacheValue.Value.ToString() ?? string.Empty);
            await this._next(context);
        }

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
