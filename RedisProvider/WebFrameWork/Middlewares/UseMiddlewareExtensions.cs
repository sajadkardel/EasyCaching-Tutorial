using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace RedisProvider.WebFrameWork.Middlewares
{
    public static class UseMiddlewareExtensions
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
}
