using Meta.Common.Applications.Handlers.Abstract;
using Meta.Example.Applications.AppServices.Contexts.Forecast.Services;

namespace Meta.Example.Applications.Handlers.Contexts.Forecast.Commands.UpdateTemperature
{
    /// <summary>
    /// Обработчик запроса <see cref="UpdateTemperatureWeatherForecastRequest" />
    /// </summary>
    /// <param name="_forecastService">Сервис предсказаний.</param>
    public class UpdateTemperatureWeatherForecastRequestHandler(IForecastService _forecastService)
        : ICommandHandler<UpdateTemperatureWeatherForecastRequest>
    {
        /// <inheritdoc/>
        public Task Handle(UpdateTemperatureWeatherForecastRequest request, CancellationToken cancellationToken)
        {
            return _forecastService.UpdateTemperatureAsync(request.Date, request.TemperatureC, cancellationToken);
        }
    }
}
