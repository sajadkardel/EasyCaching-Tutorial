using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyCaching.Core;
using InMemoryProvider.Services;

namespace InMemoryProvider.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IDateTimeService _dateTimeService;

        public ValuesController(IDateTimeService dateTimeService)
        {
            _dateTimeService = dateTimeService;
        }

        [HttpGet("GetTime")]
        public IActionResult GetTime()
        {
            return Ok(_dateTimeService.NowSecond());
        }

    }
}
