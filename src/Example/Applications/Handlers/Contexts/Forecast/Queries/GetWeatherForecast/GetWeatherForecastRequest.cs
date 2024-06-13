using Meta.Common.Contracts.Abstract;
using Meta.Example.Contracts.Forecast.Response;

namespace Meta.Example.Applications.Handlers.Contexts.Forecast.Queries.GetWeatherForecast
{
    /// <summary>
    /// Запрос на получение прогноза погоды.
    /// </summary>
    public class GetWeatherForecastRequest : Query<IReadOnlyCollection<WeatherForecastDto>>
    {
    }
}
