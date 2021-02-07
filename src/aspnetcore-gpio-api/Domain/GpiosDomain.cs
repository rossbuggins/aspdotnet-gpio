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

        private void UpdateDomainStateAndHistoryTo(GpiosDomainData from, GpiosDomainData to)
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
            ProcessStateChange(from=>
            {
                CheckExists(from, number);
                return from.UpdateState(number, state);
            });
        }

        public void ProcessStateChange (Func<GpiosDomainData,GpiosDomainData> x)
        {
            var from = gpiosDomainState;
            var to = x(from);
            UpdateDomainStateAndHistoryTo(from, to);
        }

        public void CheckExists(GpiosDomainData state, int gpioNumber)
        {
            var gpio = state.Gpios.Where(_x => gpioNumber == _x.Number).SingleOrDefault();
            if (gpio == null)
                throw new Exception("Gpio not found");
        }


    }
}