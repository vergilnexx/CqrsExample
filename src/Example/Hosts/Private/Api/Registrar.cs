using Meta.Example.Applications.AppServices.Contexts.Forecast.Services;
using Meta.Example.Infrastructures.DataAccess.Common.Configurators;
using Meta.Example.Infrastructures.DataAccess.Contexts;
using System.Reflection;
using Meta.Common.Infrastructures.DataAccess.Extensions;
using Meta.Common.Cqrs.Extensions;
using Meta.Common.Applications.AppServices.Extensions;
using Meta.Common.Applications.Handlers.Extensions;
using Meta.Example.Applications.Handlers.Contexts.Forecast.Commands.DeleteWeatherForecast;
using Meta.Common.Hosts.Features.AppFeatures.Base;
using Meta.Example.Private.Hosts.Api.Controllers;
using Meta.Common.Hosts.Consumer;
using Meta.Common.Hosts.Extensions;
using Meta.Example.Clients;

namespace Meta.Example.Private.Hosts.Api
{
    /// <summary>
    /// Регистратор.
    /// </summary>
    internal static class Registrar
    {
        /// <summary>
        /// Регистрация сервисов.
        /// </summary>
        /// <param name="services">Коллекция сервисов.</param>
        /// <param name="hostBuilder">Builder хоста.</param>
        /// <param name="loggingBuilder">Builder логгирования.</param>
        /// <param name="configuration">Конфигурация.</param>
        public static void RegistrarServices(this IServiceCollection services, 
            IHostBuilder hostBuilder, ILoggingBuilder loggingBuilder, IConfiguration configuration)
        {
            services.AddControllers();

            var currentAssembly = typeof(WeatherForecastController).Assembly;
            services
                .AddEndpointsApiExplorer()
                .AddDataAccess<ExampleDbContext, ExampleDbContextConfigurator>()
                .AddHandlers(currentAssembly)
                .AddFeatures(hostBuilder, loggingBuilder, configuration,
                [
                    currentAssembly,
                    typeof(IAppFeature).Assembly,
                    typeof(Common.Hosts.Api.Extensions.HostRegistrar).Assembly,
                    typeof(Common.Hosts.Extensions.HostRegistrar).Assembly
                ])
                .RegisterRabbitMqClient()
                .AddCommonServices()
                .AddServices()
                .AddClients(configuration);
        }

        /// <summary>
        /// Добавление обработчиков.
        /// </summary>
        /// <param name="currentAssembly">Сборка.</param>
        /// <param name="services">Коллекция сервисов.</param>
        public static IServiceCollection AddHandlers(this IServiceCollection services, Assembly currentAssembly)
        {
            services
                .AddMediatR(currentAssembly)
                .AddAssemblyHandlers(typeof(DeleteWeatherForecastRequestValidator).Assembly);
            return services;
        }

        /// <summary>
        /// Добавление общих сервисов.
        /// </summary>
        /// <param name="services">Коллекция сервисов.</param>
        public static IServiceCollection AddCommonServices(this IServiceCollection services)
        {
            services
                .AddDateTimeProvider();
            return services;
        }

        /// <summary>
        /// Добавление сервисов.
        /// </summary>
        /// <param name="services">Коллекция сервисов.</param>
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IForecastService, ForecastService>();
            return services;
        }

        /// <summary>
        /// Добавление клиента.
        /// </summary>
        /// <param name="services">Коллекция сервисов.</param>
        /// <param name="configuration">Конфигурация.</param>
        public static IServiceCollection AddClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScopedWeatherForecastServiceClient(configuration);

            return services;
        }
    }
}
