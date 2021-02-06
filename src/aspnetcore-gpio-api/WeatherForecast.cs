using System;

namespace aspnetcore_gpio
{
    public record Gpio
    {
        public Gpio(int number)
        {
            this.Number = number;
        }
       public int Number{get;set;}
    }
}

