using System;

namespace aspnetcore_gpio.Commands

{
    public class RandomGenerator
    {
        Random _random;
        public RandomGenerator()
        {
            _random=new Random();
        }

        public Random GetRandom()
        {
            return _random;
        }
    }
}