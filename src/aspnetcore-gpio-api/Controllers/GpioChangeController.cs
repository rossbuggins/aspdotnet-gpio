using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace aspnetcore_gpio.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GpioChangeController : ControllerBase
    {
     
        private readonly ILogger<GpioChangeController> _logger;

        public GpioChangeController(ILogger<GpioChangeController> logger)
        {
            _logger = logger;
        }

        [HttpGet()]
        public IEnumerable<GpioChange> Get()
        {
           return Enumerable.Range(1,12).Select(_x => new GpioChange(Guid.NewGuid(),_x));
        }

        [HttpGet("/Gpio/{number}/Change/{id}")]
        public GpioChange Get(int number, Guid id)
        {
           return new GpioChange(id,number);
        }

        [HttpPost("/Gpio/{number}/Change")]
        public async Task<IActionResult> Post(int number, GpioChange gpioChange)
        {
            if(gpioChange.Number!=null)
                throw new Exception("Number should be empty");

            if(gpioChange.ChangeId!=null)
                throw new Exception("Id should be empty");

            var changeId = Guid.NewGuid();
            var changed = gpioChange with {ChangeId = changeId, Number = number};
            await Task.Yield();
            return CreatedAtAction("GetGpio", number, changed);
        }
    }
}