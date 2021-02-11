using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspnetcore_gpio.Commands;
using aspnetcore_gpio.Controllers;
using aspnetcore_gpio.States;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using static Microsoft.AspNet.OData.Query.AllowedQueryOptions;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IO;
using System.Reflection;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using MassTransit;

namespace aspnetcore_gpio
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddHealthChecks();

            services.Configure<HealthCheckPublisherOptions>(options =>
            {
                options.Delay = TimeSpan.FromSeconds(2);
                options.Predicate = (check) => check.Tags.Contains("ready");
            });

            services.AddMassTransit(x =>
         {
             x.AddConsumer<EventConsumer>();
             x.SetKebabCaseEndpointNameFormatter();

             x.UsingRabbitMq((context, cfg) =>
             {
                 cfg.ConfigureEndpoints(context);
             });
         });

            services.AddMassTransitHostedService();

            services.AddControllers();
            services.AddApiVersioning(options =>

           {
               options.AssumeDefaultVersionWhenUnspecified = true;
               options.ReportApiVersions = true;
           });
            services.AddOData().EnableApiVersioning();

            services.AddODataApiExplorer(
                            options =>
                            {
                                // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                                // note: the specified format code will format the version as "'v'major[.minor][-status]"
                                options.GroupNameFormat = "'v'VVV";

                                // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                                // can also be used to control the format of the API version in route templates
                                options.SubstituteApiVersionInUrl = true;


                            });

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen(
                options =>
                {
                    // add a custom operation filter which sets default values
                    options.OperationFilter<SwaggerDefaultValues>();

                    // integrate xml comments
                    options.IncludeXmlComments(XmlCommentsFilePath);
                });


            services.AddSingleton<RandomGenerator>();
            services.AddSingleton<GpiosStateStorage>();
            services.AddSingleton<GpioChangesState>();

            services.AddCommandHandlers(o =>
            {
                o.Add<GpioChangeCommand, GpioChange, GpioChangeCommandHandler>();
            });


        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
        IWebHostEnvironment env,
        VersionedODataModelBuilder modelBuilder
        , IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();



                // app.UseSwagger();
                //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "aspnetcore_gpio v1"));
            }

            // app.UseHttpsRedirection();


            app.UseRouting();

            app.UseEndpoints(endpoints =>
                     {

                         endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions()
                         {
                             Predicate = (check) => check.Tags.Contains("ready"),
                         });

                         endpoints.MapHealthChecks("/health/live", new HealthCheckOptions());

                         endpoints.MapControllers();
                         // endpoints.MapControllers();
                         endpoints.Count();
                         endpoints.MapVersionedODataRoute("odata", "api", modelBuilder);
                     });

            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    // build a swagger endpoint for each discovered API version
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    }
                });

            app.UseAuthorization();


        }

        static string XmlCommentsFilePath
        {
            get
            {
                var basePath = System.AppContext.BaseDirectory;
                var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
                return Path.Combine(basePath, fileName);
            }
        }
    }

    // public class GpioDtoClass
    // {
    //     public GpioDtoClass()
    //     {

    //     }

    //     public GpioDtoClass(int number)
    //     {
    //         this.Key = number;
    //     }
    //     public int Key{get;set;}
    // }
}
