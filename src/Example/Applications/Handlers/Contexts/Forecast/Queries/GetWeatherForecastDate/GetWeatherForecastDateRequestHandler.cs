using Meta.Common.Applications.Handlers.Abstract;
using Meta.Example.Applications.AppServices.Contexts.Forecast.Services;
using Meta.Example.Contracts.Forecast.Response;

namespace Meta.Example.Applications.Handlers.Contexts.Forecast.Queries.GetWeatherForecastDate
{
    /// <summary>
    /// Обработчик запроса <see cref="GetWeatherForecastDateRequest" />
    /// </summary>
    /// <param name="_forecastService">Сервис предсказаний.</param>
    public class GetWeatherForecastDateRequestHandler(IForecastService _forecastService)
        : IQueryHandler<GetWeatherForecastDateRequest, WeatherForecastDto?>
    {
        /// <inheritdoc/>
        public Task<WeatherForecastDto?> Handle(GetWeatherForecastDateRequest request, CancellationToken cancellationToken)
        {
            return _forecastService.FindAsync(request.Date, cancellationToken);
        }
    }
}
