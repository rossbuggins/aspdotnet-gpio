using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Mvc;

namespace aspnetcore_gpio
{
    public class GpioModelConfigurationConfiguration : IModelConfiguration
    {
        /// <inheritdoc />
        public void Apply( ODataModelBuilder builder, ApiVersion apiVersion, 
            string routePrefix )
        {
            {
                builder.Namespace = "NS";

                var gpio = builder.EntitySet<Gpio>("Gpios").EntityType;
                gpio.Count().Page(1000, 1000).OrderBy().Select().Filter();
                gpio.HasKey(p => p.Number);
                gpio.Property(p => p.State);
                gpio.Collection.Function("StateF").Returns<string>();

                var change = builder.EntitySet<GpioChange>("GpioStateChangeRequests").EntityType;
                change.Count().Page(1000, 1000).OrderBy().Select().Filter();
                change.HasKey(p => p.ChangeId);
                change.Property(p => p.Complete);
                change.Property(p => p.Enabled);
                change.Property(p => p.NewOutputState);
                change.Property(p => p.Number);


                gpio.HasMany(c => c.GpioStateChangeRequests);

                
               
            }
            
        }
    }
}
