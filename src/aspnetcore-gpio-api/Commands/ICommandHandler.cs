namespace aspnetcore_gpio.Commands

{
    public interface ICommandHandler<U> 
        {
            void Handle(ICommand<U> command);
        }
}