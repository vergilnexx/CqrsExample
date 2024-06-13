using Meta.Common.Hosts.RabbitMq.Registrar.Client;
using Meta.Example.Contracts.Forecast.Events.AddedNotification;

namespace Meta.Example.Applications.Handlers.Contexts.Forecast.Commands.AddWeatherForecast
{
    /// <summary>
    /// Обработчик диагностики запроса <see cref="AddWeatherForecastRequest"/>
    /// </summary>
    public class WeatherForecastAddedNotificationMessageEventHandler 
        : Common.Applications.Handlers.Abstract.EventHandler<WeatherForecastAddedNotificationMessage>
    {
        private readonly IRabbitMqClient _rabbitMqClient;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="rabbitMqClient">Клиент RabbitMq.</param>
        public WeatherForecastAddedNotificationMessageEventHandler(IRabbitMqClient rabbitMqClient)
        {
            _rabbitMqClient = rabbitMqClient;
        }

        /// <inheritdoc/>>
        public override Task HandleAsync(WeatherForecastAddedNotificationMessage @event, CancellationToken cancellationToken)
        {
            return _rabbitMqClient.PublishAsync(@event, cancellationToken);
        }
    }
}
