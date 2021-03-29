using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyCaching.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace RedisProvider.WebFrameWork.Middlewares
{
    public static class CacheMiddleware
    {
        public static IApplicationBuilder UseCaching(this IApplicationBuilder app)
        {
            return app.UseMiddleware<CachingMiddleware>();
        }
    }

    public class CachingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IEasyCachingProvider _easyCachingProvider;

        public CachingMiddleware(RequestDelegate next, IEasyCachingProvider easyCachingProvider)
        {
            this._next = next;
            _easyCachingProvider = easyCachingProvider;
        }

        public async Task Invoke(HttpContext context)
        {
            var cacheKey = context.Request.GenerateCacheKeyFromRequest();

            if (!await _easyCachingProvider.ExistsAsync(cacheKey))
            {
                await using var swapStream = new MemoryStream();
                var originalResponseBody = context.Response.Body;
                context.Response.Body = swapStream;
                await _next(context);
                swapStream.Seek(0, SeekOrigin.Begin);
                string responseBody = await new StreamReader(swapStream).ReadToEndAsync();
                swapStream.Seek(0, SeekOrigin.Begin);
                context.Response.Body = originalResponseBody;

                await _easyCachingProvider.SetAsync(cacheKey, responseBody, TimeSpan.FromMinutes(1));
                await context.Response.WriteAsync(responseBody);
            }
            else
            {
                var cacheValue = await _easyCachingProvider.GetAsync<string>(cacheKey);
                await context.Response.WriteAsync(cacheValue.Value);
            }

            await this._next(context);
        }
    }

    public static class CacheExtension
    {
        public static string GenerateCacheKeyFromRequest(this HttpRequest httpRequest)
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
