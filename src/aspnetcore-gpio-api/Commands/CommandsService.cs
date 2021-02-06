using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspnetcore_gpio.Domain;
using aspnetcore_gpio.States;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace aspnetcore_gpio.Commands

{
    public interface ICommandHandler
    {
        void Handle(ICommand command);
    }

    public interface ICommandHandler<T>: ICommandHandler 
        where T:ICommand
        {
            void Handle(T command);
        }

    public class GpioChangeCommandHandler : ICommandHandler<GpioChangeCommand>
    {
        GpiosState _state;
        public GpioChangeCommandHandler (GpiosState state)
        {
            _state =state;
        }
        
        public void Handle(ICommand command)
        {
            Handle((GpioChangeCommand)command);
        }

    
        public void Handle(GpioChangeCommand command)
        {
            _state.State = _state.State.UpdateState(
                command.CommandData.Number, 
                command.CommandData.NewOutputState);
        }
    }

    public class CommandsService
    {
        Dictionary<Type, ICommandHandler> commandHandlers =
            new Dictionary<Type, ICommandHandler>();

        public CommandsService(
            GpioChangeCommandHandler gpioChangeCommandHandler)
        {
            commandHandlers.Add(
                typeof(GpioChangeCommand),
                gpioChangeCommandHandler);
        }

        public void Command(ICommand command)
        {
            var handler = commandHandlers[command.GetType()];
            handler.Handle(command); 
        }
    }
}