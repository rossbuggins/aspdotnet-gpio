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
  
    public record GpioChangesDomain (IEnumerable<GpioChangeDomain> GpioChanges)
    {
         public GpioChangesDomain RemoveState(Guid id)
        {
            var gpio = GpioChanges.Where(_x => id == _x.Id).Single();

              if(gpio.Complete==true)
                throw new Exception("Cannot update after complete");

           
            return new GpioChangesDomain(
                GpioChanges
                .Where(_x => id != _x.Id)
                .OrderBy(_x=>_x.Id));
        }

        public GpioChangesDomain AddState(Guid id, int number, bool state)
        {
            var gpioUpdated = new GpioChangeDomain(id, number, state, false, false);

            return new GpioChangesDomain(
                GpioChanges
                .Append(gpioUpdated)
                .OrderBy(_x=>_x.Id));
        }

        public GpioChangesDomain UpdateState(Guid id, bool state, bool enabled, bool complete)
        {
            var gpio = GpioChanges.Where(_x => id == _x.Id).Single();

              if(gpio.Complete==true)
                throw new Exception("Cannot update after complete");

            if(enabled==false && gpio.Enabled==true)
                throw new Exception("Cannot disable after enabling change");
            
          

            var gpioUpdated = gpio with {State = state, Enabled = enabled, Complete = complete};

            return new GpioChangesDomain(
                GpioChanges.Where(_x => _x.Id!=id)
                .Append(gpioUpdated)
                .OrderBy(_x=>_x.Id));
        }
    }

    public record GpioChangeDomain (Guid Id, int Number, bool State, bool Enabled, bool Complete);
}