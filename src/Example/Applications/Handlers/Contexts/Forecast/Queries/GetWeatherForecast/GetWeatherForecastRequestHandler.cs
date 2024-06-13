using Meta.Common.Applications.Handlers.Abstract;
using Meta.Example.Applications.AppServices.Contexts.Forecast.Services;
using Meta.Example.Contracts.Forecast.Response;

namespace Meta.Example.Applications.Handlers.Contexts.Forecast.Queries.GetWeatherForecast
{
    /// <summary>
    /// Обработчик запроса <see cref="GetWeatherForecastRequest" />
    /// </summary>
    /// <param name="_forecastService">Сервис предсказаний.</param>
    public class GetWeatherForecastRequestHandler(IForecastService _forecastService) 
        : IQueryHandler<GetWeatherForecastRequest, IReadOnlyCollection<WeatherForecastDto>>
    {
        /// <inheritdoc/>
        public Task<IReadOnlyCollection<WeatherForecastDto>> Handle(GetWeatherForecastRequest request, CancellationToken cancellationToken)
        {
            return _forecastService.GetAsync(cancellationToken);
        }
    }
}
