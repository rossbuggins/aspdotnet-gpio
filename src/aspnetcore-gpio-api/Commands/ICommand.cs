namespace aspnetcore_gpio.Commands
{
  
    public interface ICommand<out T>
    {
        T CommandData{get;}
    }
}