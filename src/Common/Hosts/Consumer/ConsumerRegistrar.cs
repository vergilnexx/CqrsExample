using Meta.Common.Contracts.Events;
using Meta.Common.Hosts.RabbitMq.Options;
using Meta.Common.Hosts.RabbitMq.Registrar.Client;
using Meta.Common.Hosts.RabbitMq.Registrar.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Meta.Common.Hosts.Consumer
{
    /// <summary>
    /// Регистрация обработчиков сообщений.
    /// </summary>
    public static class ConsumerRegistrar
    {
        /// <summary>
        /// Включает клиента RabbitMq.
        /// </summary>
        /// <param name="services">Коллекция сервисов.</param>
        /// <returns>Коллекция сервисов.</returns>
        /// <exception cref="InvalidOperationException">Исключение, бросаемое когда не найдены настройки.</exception>
        public static IServiceCollection RegisterRabbitMqClient(this IServiceCollection services)
        {
            services.AddOptions<RabbitMqOptions>()
                    .Configure<IConfiguration>((options, configuration) =>
                    {
                        var section = configuration.GetSection(RabbitMqOptions.ClientSectionName);
                        if (!section.Exists())
                        {
                            throw new InvalidOperationException(
                                $"Не удалось получить настройки секции '{RabbitMqOptions.ClientSectionName}'.");
                        }
                        section.Bind(options);
                    });

            services.AddSingleton<IRabbitMqClientFactory, RabbitMqClientFactory>();
            services.AddSingleton(sp => sp.GetRequiredService<IRabbitMqClientFactory>().GetClient());

            return services;
        }

        /// <summary>
        /// Регистрация сервиса RabbitMq.
        /// </summary>
        /// <param name="services">Коллекция сервисов.</param>
        /// <returns>Коллекция сервисов.</returns>
        /// <exception cref="InvalidOperationException">Исключение, бросаемое когда не найдены настройки.</exception>
        public static IServiceCollection RegisterRabbitMqService(this IServiceCollection services)
        {
            services.AddOptions<RabbitMqOptions>()
                    .Configure<IConfiguration>((options, configuration) =>
                    {
                        var section = configuration.GetSection(RabbitMqOptions.ServiceSectionName);
                        if (!section.Exists())
                        {
                            throw new InvalidOperationException(
                                $"Не удалось получить настройки секции '{RabbitMqOptions.ServiceSectionName}'.");
                        }
                        section.Bind(options);
                    });

            services.AddSingleton<IRabbitMqServiceFactory, RabbitMqServiceFactory>();
            services.AddSingleton(sp => sp.GetRequiredService<IRabbitMqServiceFactory>().GetService());

            return services;
        }

        /// <summary>
        /// Регистрация обработчика сообщений.
        /// </summary>
        /// <typeparam name="TConsumer">Тип обработчика сообщений.</typeparam>
        /// <param name="services">Коллекция сервисов.</param>
        /// <returns>Коллекция сервисов.</returns>
        public static IServiceCollection RegisterConsumer<TConsumer>(this IServiceCollection services) 
            where TConsumer : class, INamedConsumer
        {
            services.AddSingleton<INamedConsumer, TConsumer>();

            return services;
        }
    }
}
