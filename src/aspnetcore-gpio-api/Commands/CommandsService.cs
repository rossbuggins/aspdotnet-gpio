using System.Collections.Generic;
using System.Linq;
using aspnetcore_gpio.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace aspnetcore_gpio.Commands

{
    public class CommandsService<T,U>
        where T : ICommand<U>
    {
        CommandStore<T,U> _sp;
        public CommandsService(
            CommandStore<T,U> sp)
        {
            _sp = sp;
        }

        public void Command(T command)
        {
            _sp.Commands.Enqueue(command);
        }
    }
}