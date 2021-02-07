using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using aspnetcore_gpio.Commands;
namespace aspnetcore_gpio.Domain
{

    public class GpiosDomain
    {
        private GpiosDomainData gpiosDomainState;
        private Stack<GpiosDomainData> history;

        public event EventHandler<GpiosDomainStateUpdatedEventArgs> GpiosDomainStateUpdated;

        public GpiosDomain()
        {
            history = new Stack<GpiosDomainData>();
            gpiosDomainState = new GpiosDomainData(
                new GpioDomainDatas(
                    Enumerable.Range(1, 20).Select(_x => new GpioDomainData(_x, false))));
        }

        private void UpdateStateTo(GpiosDomainData from, GpiosDomainData to)
        {
            if (from == gpiosDomainState)
            {
                gpiosDomainState = to;
                history.Push(to);
            
            OnGpiosDomainStateUpdated(
                new GpiosDomainStateUpdatedEventArgs(from, to));
            }
            
        }

        protected void OnGpiosDomainStateUpdated(GpiosDomainStateUpdatedEventArgs args)
        {
            this.GpiosDomainStateUpdated!.Invoke(this, args);
        }

        public GpiosDomainData State { get { return gpiosDomainState; } }

        public void UpdateState(int number, bool state)
        {
            var from = gpiosDomainState;
            var to = from.UpdateState(number, state);
            UpdateStateTo(from, to);

        }


    }
}