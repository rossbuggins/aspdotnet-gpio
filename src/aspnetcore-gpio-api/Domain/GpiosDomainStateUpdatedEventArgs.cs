using System;
namespace aspnetcore_gpio.Domain
{
    public class GpiosDomainStateUpdatedEventArgs :EventArgs
    {
        public GpiosDomainStateUpdatedEventArgs(GpiosDomainData from, GpiosDomainData to)
        {
            this.From = from;
            this.To = to;
        }

        public GpiosDomainData From {get;init;}
        public GpiosDomainData To {get;init;}
    }
}