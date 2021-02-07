using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using aspnetcore_gpio.Commands;
using aspnetcore_gpio.Domain;

namespace aspnetcore_gpio.States
{
    public class GpiosState
  {
      public GpiosDomain State{get;set;} = new GpiosDomain(Enumerable.Range(1,20).Select(_x => new GpioDomain(_x, false)));
  }
}