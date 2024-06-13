using Google.Protobuf.WellKnownTypes;
using Meta.Common.Contracts.Utilities.Helpers;
using Meta.Example.Private.Hosts.Grpc.Protos;

namespace Meta.Example.Clients.WeatherForecast
{
    /// <inheritdoc/>
    public class WeatherForecastServiceClient : IWeatherForecastServiceClient
    {
        private readonly WeatherForecastService.WeatherForecastServiceClient _client;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="client">Клиент.</param>
        public WeatherForecastServiceClient(WeatherForecastService.WeatherForecastServiceClient client)
        {
            _client = client;
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(DateOnly date, CancellationToken cancellationToken)
        {
            var datetime = date.ToDateTime();
            var request = new DeleteForecastRequest() { Date = Timestamp.FromDateTime(datetime) };

            await _client.DeleteAsync(request, cancellationToken: cancellationToken);
        }
    }
}
