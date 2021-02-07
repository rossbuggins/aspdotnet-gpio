using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
namespace aspnetcore_gpio.Domain
{
    public record GpiosDomainData (IEnumerable<GpioDomainData> Gpios)
    {
       
        public GpiosDomainData UpdateState(int number, bool state)
        {
            var gpio = Gpios.Where(_x => number == _x.Number).Single();
            var gpioUpdated = gpio with {State = state};

            var updatedDomain = new GpiosDomainData(
                new GpioDomainDatas(
                Gpios.Where(_x => _x.Number!=number)
                .Append(gpioUpdated)
                .OrderBy(_x=>_x.Number)));

            return updatedDomain;
        }
    }
}