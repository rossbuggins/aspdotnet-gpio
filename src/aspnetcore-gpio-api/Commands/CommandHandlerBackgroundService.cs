using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace aspnetcore_gpio.Commands

{
    public class CommandHandlerBackgroundService<T,U> : BackgroundService
        where T : ICommand<U>
    {
        CommandStore<T,U> _store;
        CommandFactory _commandFactory;
        RandomGenerator _random;

        IServiceProvider _sp;
        public CommandHandlerBackgroundService(IServiceProvider sp,
         CommandStore<T,U> store, CommandFactory commandFactory, RandomGenerator random)
        {
            _store = store;
            _commandFactory = commandFactory;
            _random = random;
            _sp = sp;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
           
                await Task.Yield();
                if (_store.Commands.TryDequeue(out var command))
                {
                    var handler = _commandFactory.Create<T,U>(command);
                    var waitTime = _random.GetRandom().Next(5000,15000);
                    await Task.Delay(waitTime);
                    handler.Handle(command);
                }
                else
                {
                    await Task.Delay(1000);
                }
            }
        }
    }
}