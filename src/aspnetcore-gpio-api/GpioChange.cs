namespace aspnetcore_gpio
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;

    public record GpioChange
    (
        long? ChangeId,
        int? Number,
        [Required] bool NewOutputState
    );

}

