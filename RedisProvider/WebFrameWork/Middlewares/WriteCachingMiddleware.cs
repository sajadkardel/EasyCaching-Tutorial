using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyCaching.Core;
using Microsoft.AspNetCore.Http;

namespace RedisProvider.WebFrameWork.Middlewares
{
    public class WriteCachingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IEasyCachingProvider _easyCachingProvider;

        public WriteCachingMiddleware(RequestDelegate next, IEasyCachingProvider easyCachingProvider)
        {
            this.next = next;
            _easyCachingProvider = easyCachingProvider;
        }

        public async Task Invoke(HttpContext context)
        {
            await _easyCachingProvider.SetAsync(GenerateCacheKeyFromRequest(context.Request), 12, TimeSpan.FromMinutes(40));
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
