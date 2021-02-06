using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace aspnetcore_gpio.Commands

{
    public record GpioChangeCommand:ICommand<GpioChange>
    {
        public GpioChange CommandData{get; init;}
    }
}