using aspnetcore_gpio.States;

namespace aspnetcore_gpio.Commands

{
    public class GpioChangeCommandHandler : 
        ICommandHandler<GpioChange>
    {
        GpiosState _state;
        public GpioChangeCommandHandler (GpiosState state)
        {
            _state =state;
        }

        public void Handle(ICommand<GpioChange> command)
        {
             _state.State = _state.State.UpdateState(
                command.CommandData.Number, 
                command.CommandData.NewOutputState);
        }

  
    }
}