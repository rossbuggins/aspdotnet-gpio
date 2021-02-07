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
    [ODataRoutePrefix("Gpios")] 
    [ODataRouting()] 
     [ControllerName( "Gpios" )]
    public partial class MyGpiosController : ODataController
    {
        private readonly ILogger<MyGpiosController> _logger;
        private readonly CommandsService<GpioChangeCommand, GpioChange> _command;

        GpiosStateStorage _state;
        GpioChangesState _commandState;
        public MyGpiosController(ILogger<MyGpiosController> logger,
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
                .Select(_x => 
                    new Gpio(
                        _x.Number, 
                        _x.State, 
                       new List<GpioChange>()))
                .ToList();

            return data.AsQueryable();
        }

        // GET ~/api/v1/orders/{key}?api-version=1.0
      //  [HttpGet("{key:int}")]
       [ODataRoute("({id})")]
        //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Gpio))]
        // [EnableQuery(
        //   AllowedQueryOptions = Select)]
        public Gpio Get(int id)
        {
            var data = _state.Domain.State.Gpios
                .Where(_x => _x.Number == id)
                .Select(_x => new Gpio(_x.Number, _x.State, new List<GpioChange>()))
              .First();

            return data;
        }

      [ODataRoute("({id})/GpioStateChangeRequests")]
        public IQueryable<GpioChange> GetGpioStateChangeRequests(int id)
        
        {
            var data =
            _commandState.State.GpioChanges
            .Where(_x=>_x.Number==id)
             .Select(_x => new GpioChange(
                 _x.Id, _x.Number, _x.State, _x.Enabled, _x.Complete))
             .ToList();

            return data.AsQueryable();
        }

        [ODataRoute("NS.StateF()")]
        public string StateF(int gpioId)
        {
            return "raaa";
        }

        [ODataRoute("({gpioId})/GpioStateChangeRequests({changeId})")]
        public IQueryable<GpioChange> GetGpioStateChangeRequests(int gpioId,Guid changeId)
        
        {
            var data =
            _commandState.State.GpioChanges
            .Where(_x=>_x.Number==gpioId && _x.Id==changeId)
             .Select(_x => new GpioChange(
                 _x.Id, _x.Number, _x.State, _x.Enabled, _x.Complete))
             .ToList();

            return data.AsQueryable();
        }

      //  [ODataRoute("Gpios({key})/GpioChanges")]
      //      public bool GetGpioChanges2(int key)
      //  {
      //      return false;
       // }

        [EnableQuery]
        [ODataRoute("Gpios({key})/A({key2})")]
        [HttpGet("Gpios/{key}/A/{key2}")]
        public Gpio GetGpio2(int key, int key2)
        {
            throw new NotImplementedException();
        }


        [ODataRoute("Gpios/Blank")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Gpio))]
         [EnableQuery(
            AllowedQueryOptions = Select)]
        public SingleResult<Gpio> GetBlank()
        {
            var data = new List<Gpio>();
            data.Add(new Gpio(0, false, new List<GpioChange>()));

            return SingleResult.Create<Gpio>(data.AsQueryable());

        }
    }


[ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class MyController : ODataController
{
    [HttpGet]
    [ODataRoute("Gpios({id})/State")]
    public bool GetStateOfGpio([FromODataUri]int id)
    {
            return false;
        }
}
}