using System.Collections.Concurrent;

namespace aspnetcore_gpio.Commands

{
    public class CommandStore<T,U> where T : ICommand<U>
    {
        public ConcurrentQueue<T> Commands = 
            new ConcurrentQueue<T>();
   
    }
}