using System.Collections.Generic;
using aspnetcore_gpio.Domain;

namespace aspnetcore_gpio.States
{
    public class GpioChangesState
    {
        public GpioChangesData State { get; set; } = new GpioChangesData(new List<GpioChangeDomain>());
    }
}