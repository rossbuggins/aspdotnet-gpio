using aspnetcore_gpio.States;

namespace aspnetcore_gpio.Commands

{
    public class GpioChangeCommandHandler : 
        ICommandHandler<GpioChange>
    {
        GpiosStateStorage _state;
        public GpioChangeCommandHandler (GpiosStateStorage state)
        {
            _state =state;
        }

        public void Handle(ICommand<GpioChange> command)
        {
              _state.Domain.UpdateState(
                command.CommandData.Number, 
                command.CommandData.NewOutputState);
        }

  
    }
}