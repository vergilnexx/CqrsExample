using Meta.Common.Contracts.Abstract;
using Meta.Example.Contracts.Forecast.Response;

namespace Meta.Example.Applications.Handlers.Contexts.Forecast.Queries.GetWeatherForecastDate
{
    /// <summary>
    /// Запрос на получение прогноза погоды в конкретную дату.
    /// </summary>
    /// <param name="date">Дата.</param>
    public class GetWeatherForecastDateRequest(DateOnly date) : Query<WeatherForecastDto?>
    {
        /// <summary>
        /// Дата.
        /// </summary>
        public DateOnly Date { get; } = date;
    }
}
