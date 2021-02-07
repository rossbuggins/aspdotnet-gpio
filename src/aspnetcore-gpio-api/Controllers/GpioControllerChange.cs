using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using aspnetcore_gpio.Commands;
namespace aspnetcore_gpio.Controllers
{

    public partial class GpioController
    {

        [HttpGet("{number}/stateChangeRequests")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GpioChange>))]
        public IActionResult GetGpioChanges()
        {
            return
             new OkObjectResult(_commandState.State.GpioChanges
             .Select(_x => new GpioChange(_x.Id, _x.Number, _x.State, _x.Enabled, _x.Complete)));
        }

        [HttpGet("{number}/stateChangeRequests/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GpioChange))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetGpioChange(int number, Guid id)
        {
            var r = _commandState.State.GpioChanges.Where(_x => _x.Id == id)
            .Select(_x => new GpioChange(_x.Id, _x.Number, _x.State, _x.Enabled, _x.Complete)).SingleOrDefault();

            if (r == null)
                return new NotFoundObjectResult(new { message = "Invalid gpio number" });

            return Ok(r);
        }

        [HttpDelete("{number}/stateChangeRequests/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GpioChange))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteGpioChange(
            [FromRoute] int number,
            [FromRoute] Guid id)
        {
            var gpioChange = _commandState.State.GpioChanges.Where(_x => _x.Id == id).SingleOrDefault();

            if (gpioChange == null)
                return new NotFoundObjectResult(new { message = "Invalid gpio number" });

            if (gpioChange.Complete)
                return new BadRequestObjectResult(new { message = "Cannot update after complete" });

       

            _commandState.State = _commandState.State.RemoveState(
                             gpioChange.Id);

            await Task.Yield();
            return Ok(gpioChange);
        }

        [HttpPut("{number}/stateChangeRequests/{id}")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(GpioChange))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutGpioChange(
            [FromRoute] int number,
            [FromRoute] Guid id,
            [FromBody] UpdateGpioChange gpioChangeEnabled)
        {
            var gpioChange = _commandState.State.GpioChanges.Where(_x => _x.Id == id).SingleOrDefault();

            if (gpioChange == null)
                return new NotFoundObjectResult(new { message = "Invalid gpio number" });

            if (gpioChange.Complete)
                return new BadRequestObjectResult(new { message = "Cannot update after complete" });

            if (gpioChange.Enabled && gpioChangeEnabled.Enabled==false)
                return new BadRequestObjectResult(new { message = "Cannot disable after enabling" });


            var changed = new GpioChange(
                gpioChange.Id, 
                gpioChange.Number, 
                gpioChangeEnabled.NewOutputState, 
                gpioChangeEnabled.Enabled,
                false);

            _commandState.State = _commandState.State.UpdateState(
                            changed.ChangeId,
                            changed.NewOutputState,
                            changed.Enabled,
                            changed.Complete);

            //this should then give event  "Upate State Event "


            await Task.Yield();

            //this should be listening for that pub sub style
            _command.Command(new GpioChangeCommand() { CommandData = changed });


            //this is then the eventaul consistency - this should listen to something
            // from the actual domain to know all is done 
             _commandState.State = _commandState.State.UpdateState(
                            changed.ChangeId,
                            gpioChangeEnabled.NewOutputState,
                        gpioChangeEnabled.Enabled,
                            true);


            return AcceptedAtAction(
               "GetGpio",
               new { number = changed.Number },
               changed);
        }

        [HttpPost("{number}/stateChangeRequests")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(GpioChange))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> PostGpioChange(
            [FromRoute] int number,
            [FromBody] NewGpioChange gpioChange)
        {

            if (_state.Domain.State.Gpios.Where(_x => _x.Number == number).SingleOrDefault() == null)
                return new NotFoundObjectResult(new { message = "Invalid gpio number" });

            var changeId = Guid.NewGuid();
            var change = new GpioChange(changeId, number, gpioChange.NewOutputState.Value, false, false);

            _commandState.State = _commandState.State.AddState(
                change.ChangeId,
                change.Number,
                change.NewOutputState);

            await Task.Yield();

            return AcceptedAtAction(
                "GetGpioChange",
                new { number = change.Number, id = change.ChangeId },
                change);
        }
    }
}