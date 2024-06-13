using MassTransit;
using Meta.Common.Contracts.Events;
using Meta.Common.Contracts.Utilities.Helpers;
using Meta.Example.Contracts.Forecast.Events.AddedNotification;
using Meta.Example.Contracts.Forecast.Events.Delete;
using Microsoft.Extensions.Logging;

namespace Meta.Example.Hosts.Consumer.Consumers
{
    /// <summary>
    /// Обработчик сообщения <see cref="IDeleteWeatherForecastMessage"/>
    /// </summary>
    public sealed class WeatherForecastAddedNotificationConsumer(ILogger<WeatherForecastAddedNotificationConsumer> logger) 
        : INamedConsumer<IWeatherForecastAddedNotificationMessage>
    {
        /// <inheritdoc/>
        public string QueueName => typeof(IWeatherForecastAddedNotificationMessage).FullName!;

        /// <inheritdoc/>
        public Task Consume(ConsumeContext<IWeatherForecastAddedNotificationMessage> context)
        {
            logger.LogInformation("Уведомление: создан прогноз на дату: {Date}", 
                context.Message.Date.ToDateTime().ToLongDateString());
            return Task.CompletedTask;
        }
    }
}
