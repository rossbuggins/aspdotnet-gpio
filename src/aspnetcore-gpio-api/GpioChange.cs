namespace aspnetcore_gpio
{
    using System;
    using System.Text.Json.Serialization;

    public record GpioChange
    {

        public GpioChange()
        {

        }
        public GpioChange(int number)
        {
            this.Number = number;
        }

     
           public GpioChange(Guid changeId, int number)
        {
            this.ChangeId = changeId;
            this.Number = number;
        }
        public Guid ? ChangeId{get;set;}
       public int ? Number{get;set;}
    }
}

