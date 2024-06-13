using MassTransit;
using Meta.Common.Contracts.Events;
using Meta.Example.Clients.WeatherForecast;
using Meta.Example.Contracts.Forecast.Events.Delete;

namespace Meta.Example.Hosts.Consumer.Consumers
{
    /// <summary>
    /// Обработчик сообщения <see cref="IDeleteWeatherForecastMessage"/>
    /// </summary>
    public sealed class DeleteWeatherForecastConsumer(IWeatherForecastServiceClient _weatherForecastServiceClient) : INamedConsumer<IDeleteWeatherForecastMessage>
    {
        /// <inheritdoc/>
        public string QueueName => typeof(IDeleteWeatherForecastMessage).FullName!;

        /// <inheritdoc/>
        public Task Consume(ConsumeContext<IDeleteWeatherForecastMessage> context)
        {
            return _weatherForecastServiceClient.DeleteAsync(context.Message.Date, context.CancellationToken);
        }
    }
}
