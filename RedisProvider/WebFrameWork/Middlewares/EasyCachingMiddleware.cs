using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyCaching.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using RedisProvider.Services;

namespace RedisProvider.WebFrameWork.Middlewares
{
    public static class EasyCachingMiddlewareExtensions
    {
        public static IApplicationBuilder UseEasyCaching(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<EasyCachingMiddleware>();
        }
    }

    public class EasyCachingMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly ICacheService _cacheService;
        public EasyCachingMiddleware(RequestDelegate next, ICacheService cacheService)
        {
            _next = next;
            _cacheService = cacheService;
        }

        public async Task Invoke(HttpContext context)
        {
          
        }
    }
}
