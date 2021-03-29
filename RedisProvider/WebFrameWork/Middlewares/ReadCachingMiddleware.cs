using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyCaching.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;

namespace RedisProvider.WebFrameWork.Middlewares
{
    public class ReadCachingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IEasyCachingProvider  _easyCachingProvider;

        public ReadCachingMiddleware(RequestDelegate next, IEasyCachingProvider easyCachingProvider)
        {
            this.next = next;
            _easyCachingProvider = easyCachingProvider;
        }

        public async Task Invoke(HttpContext context)
        {
            var cacheValue = await _easyCachingProvider.GetAsync<string>(GenerateCacheKeyFromRequest(context.Request));
            await context.Response.WriteAsync(cacheValue.Value);
            await this.next(context);
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
