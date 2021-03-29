using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using EasyCaching.Core;
using Newtonsoft.Json;

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
            return Ok(DateTime.Now.Second);
        }
    }
}
