using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HybridProvider.WebFrameWork.CachedAttribute;

namespace HybridProvider.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet("GetTime")]
        [Cached]
        public IActionResult GetTime()
        {
            return Ok(DateTime.Now.Second);
        }
    }
}
