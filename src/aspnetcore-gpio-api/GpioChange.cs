namespace aspnetcore_gpio
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;

    public record GpioChange(Guid ChangeId, int Number,bool NewOutputState, bool Enabled, bool Complete);

 public record UpdateGpioChange(bool NewOutputState, bool Enabled);


     public record NewGpioChange([Required] bool NewOutputState);

}

