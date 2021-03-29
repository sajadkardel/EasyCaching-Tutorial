using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyCaching.Core;

namespace RedisProvider.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IEasyCachingProvider _easyCachingProvider;

        public ValuesController(IEasyCachingProvider easyCachingProvider)
        {
            _easyCachingProvider = easyCachingProvider;
        }

        [HttpGet("GetTime")]
        public IActionResult GetTime()
        {
            var cache = _easyCachingProvider.Get<int>($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod()?.Name}");
            if (cache.HasValue)
            {
                return Ok(cache.Value);
            }

            var data = DateTime.Now.Second;
            _easyCachingProvider.Set($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod()?.Name}", data, TimeSpan.FromMinutes(1));
            return Ok(data);
        }
    }
}
