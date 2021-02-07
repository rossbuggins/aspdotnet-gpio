using System;
using Microsoft.Extensions.DependencyInjection;

namespace aspnetcore_gpio.Commands

{
    public class CommandFactory
    {
        IServiceProvider _sp;
        public CommandFactory(
            IServiceProvider sp)
        {
            _sp = sp;
        }

        public ICommandHandler<U> Create<T, U>(T command)
            where T : ICommand<U>
        {
            try
            {
                var commandDataType = command.CommandData.GetType();
                var openGenericCommand = typeof(ICommand<>);
                var genericCommand = openGenericCommand.MakeGenericType(commandDataType);

                var openGenericHandler = typeof(ICommandHandler<>);
                var t1 = command.GetType();
    
                var type =openGenericHandler.MakeGenericType(commandDataType);

                var handler = (ICommandHandler<U>)_sp.GetRequiredService(type);
                return handler;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}