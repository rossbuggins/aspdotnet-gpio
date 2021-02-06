using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using aspnetcore_gpio.Commands;
namespace aspnetcore_gpio.Domain
{
  
    public record GpiosDomain (IEnumerable<GpioDomain> Gpios)
    {
        public GpiosDomain UpdateState(int number, bool state)
        {
            var gpio = Gpios.Where(_x => number == _x.Number).Single();
            var gpioUpdated = gpio with {State = state};

            return new GpiosDomain(
                Gpios.Where(_x => _x.Number!=number)
                .Append(gpioUpdated)
                .OrderBy(_x=>_x.Number));
        }
    }

    public record GpioDomain (int Number, bool State);
}