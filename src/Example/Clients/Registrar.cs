using Grpc.Core;
using Meta.Common.Contracts.Options;
using Meta.Example.Clients.Options;
using Meta.Example.Clients.WeatherForecast;
using Meta.Example.Private.Hosts.Grpc.Protos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Meta.Example.Clients
{
    /// <summary>
    /// Регистрация клиентов.
    /// </summary>
    public static class Registrar
    {
        /// <summary>
        /// Добавление клиента предсказания погоды.
        /// </summary>
        /// <param name="services">Коллекция сервисов.</param>
        /// <param name="configuration">Конфигурация.</param>
        /// <returns>Коллекция сервисов.</returns>
        public static IServiceCollection AddScopedWeatherForecastServiceClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IWeatherForecastServiceClient, WeatherForecastServiceClient>();
            AddWeatherForecastServiceGrpcClient(services, configuration);

            return services;
        }

        /// <summary>
        /// Добавление клиента предсказания погоды.
        /// </summary>
        /// <param name="services">Коллекция сервисов.</param>
        /// <param name="configuration">Конфигурация.</param>
        /// <returns>Коллекция сервисов.</returns>
        public static IServiceCollection AddSingletonWeatherForecastServiceClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IWeatherForecastServiceClient, WeatherForecastServiceClient>();
            AddWeatherForecastServiceGrpcClient(services, configuration);

            return services;
        }

        private static void AddWeatherForecastServiceGrpcClient(IServiceCollection services, IConfiguration configuration)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            services
                .AddOptions<WeatherForecastClientOptions>()
                .Configure<IConfiguration>((options, configuration) =>
                {
                    var section = configuration.GetSection(WeatherForecastClientOptions.SectionName);
                    if (!section.Exists())
                    {
                        throw new InvalidOperationException(
                            $"Не удалось получить настройки секции '{WeatherForecastClientOptions.SectionName}'.");
                    }
                    section.Bind(options);
                });

            services
                .AddGrpcClient<WeatherForecastService.WeatherForecastServiceClient>(o =>
                {
                    var section = configuration.GetSection(WeatherForecastClientOptions.SectionName);
                    if (!section.Exists())
                    {
                        throw new InvalidOperationException(
                            $"Не удалось получить настройки секции '{WeatherForecastClientOptions.SectionName}'.");
                    }

                    var options = section.Get<WeatherForecastClientOptions>();
                    if (string.IsNullOrEmpty(options?.Url))
                    {
                        throw new InvalidOperationException(
                            $"Не удалось получить настройки секции '{WeatherForecastClientOptions.SectionName}'.");
                    }

                    o.Address = new Uri(options.Url, uriKind: UriKind.Absolute);
                })
                .ConfigureChannel(o =>
                {
                    CallCredentials credentials = CallCredentials.FromInterceptor((context, metadata) =>
                    {
                        metadata.Add(HttpRequestHeader.Authorization.ToString(),
                            $"{TrustedNetworkOptions.Scheme} {TrustedNetworkOptions.Service}");
                        return Task.CompletedTask;
                    });

                    o.Credentials = ChannelCredentials.Create(ChannelCredentials.Insecure, credentials);
                    o.UnsafeUseInsecureChannelCallCredentials = true;
                });
        }
    }
}
