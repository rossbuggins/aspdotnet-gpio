using aspnetcore_gpio.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace System

{
    public static class CommandHandlerExtensionMethods
    {
        public static IServiceCollection AddCommandHandler<T, U, V>(
            this IServiceCollection services)
            where T : ICommand<U>
            where V : class, ICommandHandler<U>
        {
            services.AddHostedService<CommandHandlerBackgroundService<T, U>>();
            services.AddTransient<ICommandHandler<U>, V>();
            return services;
        }

            public static IServiceCollection AddCommandHandlers(
            this IServiceCollection services, Action<CommandHanderOptions> options)
        {
            var o = new CommandHanderOptions(services);
            services.AddSingleton<CommandFactory>();
            services.AddHostedService<GpioStateChangedService>();
            services.AddSingleton(typeof(CommandStore<,>), typeof(CommandStore<,>));
            services.AddSingleton(typeof(CommandsService<,>), typeof(CommandsService<,>));
            options(o);

            return services;
        }
    }

    public class CommandHanderOptions
    {
        IServiceCollection services;

        public CommandHanderOptions(IServiceCollection services)
        {
            this.services = services;
        }
        public IServiceCollection Add<T, U, V>()
            where T : ICommand<U>
            where V : class, ICommandHandler<U>
            {
                services.AddHostedService<CommandHandlerBackgroundService<T, U>>();
                services.AddTransient<ICommandHandler<U>, V>();
                return this.services;
            }
    }
}