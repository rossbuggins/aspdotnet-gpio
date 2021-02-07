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

namespace aspnetcore_gpio.Controllers
{

    /// <summary>
    /// Gpio Controller. This is test stuff.
    /// </summary>
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Page(MaxTop = 100)]
    public partial class GpiosController : ODataController
    {
        private readonly ILogger<GpiosController> _logger;
        private readonly CommandsService<GpioChangeCommand, GpioChange> _command;

        GpiosStateStorage _state;
        GpioChangesState _commandState;
        public GpiosController(ILogger<GpiosController> logger,
            CommandsService<GpioChangeCommand, GpioChange> command,
            GpiosStateStorage state,
            GpioChangesState commandState)
        {
            _logger = logger;
            _command = command;
            _state = state;
            _commandState = commandState;
        }

        [Produces("application/json")]
        [ProducesResponseType(typeof(IQueryable<Gpio>), Status200OK)]
        [EnableQuery(
            MaxTop = 100, 
            AllowedQueryOptions = Select | Top | Skip | Count | OrderBy | Filter,
            AllowedOrderByProperties = "number")]
        public IQueryable<Gpio> Get()
        {
            var data = _state.Domain.State.Gpios
                .Select(_x => new Gpio(_x.Number, _x.State))
                .ToList();

            return data.AsQueryable();
        }

        // GET ~/api/v1/orders/{key}?api-version=1.0
        [HttpGet("{key:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Gpio))]
        public IActionResult Get(int key, ODataQueryOptions<Gpio> options) =>
            Ok(new Gpio(1, true));


    }
}