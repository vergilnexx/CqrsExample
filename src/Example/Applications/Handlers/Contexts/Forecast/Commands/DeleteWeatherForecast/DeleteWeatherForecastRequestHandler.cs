using Meta.Common.Applications.Handlers.Abstract;
using Meta.Example.Applications.AppServices.Contexts.Forecast.Services;

namespace Meta.Example.Applications.Handlers.Contexts.Forecast.Commands.DeleteWeatherForecast
{
    /// <summary>
    /// Обработчик запроса <see cref="DeleteWeatherForecastRequest" />
    /// </summary>
    /// <param name="_forecastService">Сервис предсказаний.</param>
    public class DeleteWeatherForecastRequestHandler(IForecastService _forecastService)
        : ICommandHandler<DeleteWeatherForecastRequest>
    {
        /// <inheritdoc/>
        public Task Handle(DeleteWeatherForecastRequest request, CancellationToken cancellationToken)
        {
            return _forecastService.DeleteAsync(request.Date, cancellationToken);
        }
    }
}
