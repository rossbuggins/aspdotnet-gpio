namespace aspnetcore_gpio.Commands
{
    public interface ICommand
    {

    }
    public interface ICommand<T>:ICommand
    {
        T CommandData{get;}
    }
}