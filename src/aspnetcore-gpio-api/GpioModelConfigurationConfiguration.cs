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
            var person = builder.EntitySet<Gpio>("Gpios").EntityType;
            person.Count().Page(1000, 1000).OrderBy().Select().Filter();
            person.HasKey( p => p.Number );
        }
    }
}
