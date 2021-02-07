using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using aspnetcore_gpio.Domain;
using aspnetcore_gpio.States;
using JsonDiffPatchDotNet;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;

namespace aspnetcore_gpio.Commands

{
    public class GpioStateChangedService : BackgroundService
    {
        GpiosStateStorage stateStore;
        public GpioStateChangedService(GpiosStateStorage stateStore)
        {
            this.stateStore = stateStore;

        }

        private async void Updated(object sender,
        GpiosDomainStateUpdatedEventArgs args)
        {
            var jdp = new JsonDiffPatch();
            var from = JToken.FromObject(args.From);
            var to = JToken.FromObject(args.To);

            JToken patch = jdp.Diff(from,to);



            Console.WriteLine("UPDATED STATE TO ");
            Console.WriteLine(args.To);
            Console.WriteLine("UPDATED DIFF");
            Console.WriteLine(patch.ToString());

          var a = new client.GpioApiClient("http://localhost:5000/", new HttpClient());
           var ab = await a.GpiosAsync();
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.stateStore.Domain.GpiosDomainStateUpdated += Updated;
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Yield();

            }
        }
    }
}