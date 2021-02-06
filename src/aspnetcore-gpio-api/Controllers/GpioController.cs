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
    public class GpioController : ControllerBase
    {
     
        private readonly ILogger<GpioController> _logger;

        public GpioController(ILogger<GpioController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Gpio> Get()
        {
           return Enumerable.Range(1,12).Select(_x => new Gpio(_x));
        }

        [HttpGet("{id}", Name="GetGpio")]
        public Gpio Get(int id)
        {
           return new Gpio(id);
        }
    }
}