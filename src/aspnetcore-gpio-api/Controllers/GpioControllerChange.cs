using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using aspnetcore_gpio.Commands;
using aspnetcore_gpio.States;
using Microsoft.AspNet.OData;
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
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using static Microsoft.AspNet.OData.Query.AllowedQueryOptions;
using static Microsoft.AspNetCore.Http.StatusCodes;
using Microsoft.OData;
using MassTransit;
using System.Threading;

namespace aspnetcore_gpio.Controllers
{

    public partial class GpioStateChangeRequestsController :ODataController
    {
         private readonly ILogger<GpioStateChangeRequestsController> _logger;
        private readonly CommandsService<GpioChangeCommand, GpioChange> _command;
 IPublishEndpoint _publishEndpoint;
        GpiosStateStorage _state;
        GpioChangesState _commandState;

        public GpioStateChangeRequestsController(ILogger<GpioStateChangeRequestsController> logger,
            CommandsService<GpioChangeCommand, GpioChange> command,
            GpiosStateStorage state,
            GpioChangesState commandState,
             IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _command = command;
            _state = state;
            _commandState = commandState;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet()]
        [ODataRoute("Gpios({key})/GpioStateChangeRequests({changeId})")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GpioChange>))]
         [EnableQuery(
            AllowedQueryOptions = Select)]
        public IQueryable<GpioChange> GetChange([FromODataUri]int key, [FromODataUri] Guid changeId)
        {
            return new List<GpioChange>().AsQueryable();

        }

     
      [Produces("application/json")]
        [ProducesResponseType(typeof(IQueryable<GpioChange>), Status200OK)]
        [EnableQuery(
            MaxTop = 100, 
            AllowedQueryOptions = Select | Top | Skip | Count | OrderBy | Filter,
            AllowedOrderByProperties = "number")]
        public IQueryable<GpioChange> Get()
        {
            var data =
            _commandState.State.GpioChanges
             .Select(_x => new GpioChange(
                 _x.Id, _x.Number, _x.State, _x.Enabled, _x.Complete))
             .ToList();

            return data.AsQueryable();
        }

        [HttpGet("{key:uuid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GpioChange))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public SingleResult<GpioChange> Get(Guid key)
        {
            Guid id = Guid.Empty;
            var r = _commandState.State.GpioChanges.Where(_x => _x.Id == id)
            .Select(_x => new GpioChange(_x.Id, _x.Number, _x.State, _x.Enabled, _x.Complete))
            .AsQueryable();

            //if (r == null)
           //     return new NotFoundObjectResult(new { message = "Invalid gpio number" });

         return SingleResult.Create<GpioChange>(r);
        }

        [HttpDelete("{key:uuid}", Name="DeleteStateChangeRequest")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GpioChange))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteGpioChange(
            Guid key)
        {
            var gpioChange = _commandState.State.GpioChanges.Where(_x => _x.Id == key).SingleOrDefault();

            if (gpioChange == null)
                return new NotFoundObjectResult(new { message = "Invalid gpio number" });

            if (gpioChange.Complete)
                return new BadRequestObjectResult(new { message = "Cannot update after complete" });

       

            _commandState.State = _commandState.State.RemoveState(
                             gpioChange.Id);

            await Task.Yield();
            return Ok(gpioChange);
        }

        [HttpPut("{key:uuid}")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(GpioChange))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutGpioChange(
            [FromODataUri]  Guid key,
            [FromBody] UpdateGpioChange gpioChangeEnabled)
        {
            var gpioChange = _commandState.State.GpioChanges.Where(_x => _x.Id == key).SingleOrDefault();

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

        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(GpioChange))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostGpioChange(
            [FromBody] NewGpioChange gpioChange, CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                await _publishEndpoint.Publish<ChangeGpioCommandMessage>(new
                {
                    Value = new ChangeGpioCommandMessage() { Number = gpioChange.Number.Value }
                });
            }

            var number = gpioChange.Number.Value;

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


    public class ChangeGpioCommandMessage
    {
        public int Number{ get; set; }
    }

       class EventConsumer :
        IConsumer<ChangeGpioCommandMessage>
    {
        ILogger<EventConsumer> _logger;

        public EventConsumer(ILogger<EventConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ChangeGpioCommandMessage> context)
        {
            _logger.LogInformation("Number: {Value}", context.Message.Number);
        }
    }
}