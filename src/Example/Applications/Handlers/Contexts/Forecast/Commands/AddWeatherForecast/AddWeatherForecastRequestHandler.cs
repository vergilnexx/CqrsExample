using Meta.Common.Applications.Handlers.Abstract;
using Meta.Common.Cqrs.Behaviors.Events;
using Meta.Example.Applications.AppServices.Contexts.Forecast.Models;
using Meta.Example.Applications.AppServices.Contexts.Forecast.Services;
using Meta.Example.Applications.Handlers.Contexts.Forecast.Queries.GetWeatherForecastDate;
using Meta.Example.Contracts.Forecast.Events.AddedNotification;

namespace Meta.Example.Applications.Handlers.Contexts.Forecast.Commands.AddWeatherForecast
{
    /// <summary>
    /// Обработчик запроса <see cref="GetWeatherForecastDateRequest" />
    /// </summary>
    /// <param name="_forecastService">Сервис предсказаний.</param>
    /// <param name="_eventMessageProvider">Провайдер событий.</param>
    public class AddWeatherForecastRequestHandler(
        IForecastService _forecastService, 
        IEventMessageProvider _eventMessageProvider)
        : ICommandHandler<AddWeatherForecastRequest, int>
    {
        /// <inheritdoc/>
        public async Task<int> Handle(AddWeatherForecastRequest request, CancellationToken cancellationToken)
        {
            var model = new AddWeatherForecastModel(request.Date, request.TemperatureC) { Summary = request.Summary };
            var id = await _forecastService.AddAsync(model, cancellationToken);

            _eventMessageProvider.Add(new WeatherForecastAddedNotificationMessage { CorrelationId = request.CorrelationId, Date = request.Date });

            return id;
        }
    }
}
