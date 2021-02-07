using System;
using System.Collections.Generic;

namespace aspnetcore_gpio
{
    public record Gpio (int Number, bool State, IEnumerable<GpioChange> GpioStateChangeRequests);
}

