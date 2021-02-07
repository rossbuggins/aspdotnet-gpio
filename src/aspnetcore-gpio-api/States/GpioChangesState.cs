using System.Collections.Generic;
using aspnetcore_gpio.Domain;

namespace aspnetcore_gpio.States
{
    public class GpioChangesState
    {
        public GpioChangesDomain State { get; set; } = new GpioChangesDomain(new List<GpioChangeDomain>());
    }
}