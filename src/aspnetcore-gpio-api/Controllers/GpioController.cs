using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using aspnetcore_gpio.Commands;
using aspnetcore_gpio.States;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace aspnetcore_gpio.Controllers
{
    [ApiController]
    [Route("gpios")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public partial class GpioController : ControllerBase
    {   
        private readonly ILogger<GpioController> _logger;
        private readonly CommandsService<GpioChangeCommand,GpioChange> _command;

        GpiosStateStorage _state;
        GpioChangesState _commandState;
        public GpioController(ILogger<GpioController> logger,
            CommandsService<GpioChangeCommand,GpioChange> command,
            GpiosStateStorage state,
            GpioChangesState commandState)
        {
            _logger = logger;
            _command = command;
            _state = state;
            _commandState = commandState;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(IEnumerable<Gpio>))]
        public IActionResult GetGpios()
        {
            return 
            new OkObjectResult(
                _state.Domain.State.Gpios.Select(_x => new Gpio(_x.Number, _x.State)));
        }

        [HttpGet("{number}", Name="GetGpio")]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(Gpio))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetGpio(int number)
        {
            var gpio = 
            _state.Domain.State.Gpios.Where(_x=>_x.Number == number).Select(_x => new Gpio(_x.Number, _x.State)).SingleOrDefault();
              if(gpio==null)
                return new NotFoundObjectResult(new {message="Invalid gpio number"});

                
           return Ok(gpio);
        }
    }
}