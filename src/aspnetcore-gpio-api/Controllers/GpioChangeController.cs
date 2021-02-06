using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
           return Enumerable.Range(1,12).Select(_x => 
                new GpioChange(1,_x, false));
        }

        [HttpGet("/Gpio/{number}/Change/{id}")]
        public GpioChange Get(int number, long id)
        {
           return new GpioChange(id, number, false);
        }


        [HttpPost("/Gpio/{number}/Change")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post(
            [FromRoute] int number,
            [FromBody] GpioChange gpioChange)
        {
            
            if(gpioChange.Number!=null)
              return BadRequest(new {message="Number should be empty"});

            if(gpioChange.ChangeId!=null)
                return BadRequest(new {message="Id should be empty"});

            var changeId = 999;
            var changed = gpioChange with {ChangeId = changeId, Number = number};
            await Task.Yield();
            
            return CreatedAtAction(
                "Get", 
                "GpioChange",
                new {number = changed.Number, id=changed.ChangeId}, 
                changed);
        }
    }
}