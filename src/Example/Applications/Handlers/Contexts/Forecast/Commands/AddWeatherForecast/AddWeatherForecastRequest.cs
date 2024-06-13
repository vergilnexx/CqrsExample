using Meta.Common.Contracts.Abstract;
using Meta.Example.Contracts.Forecast.Requests;

namespace Meta.Example.Applications.Handlers.Contexts.Forecast.Commands.AddWeatherForecast
{
    /// <summary>
    /// Запрос на добавление прогноза.
    /// </summary>
    /// <param name="data">Данные.</param>
    public class AddWeatherForecastRequest(WeatherForecastData data) : Command<int>
    {
        /// <summary>
        /// Дата.
        /// </summary>
        public DateOnly Date { get; } = data.Date;

        /// <summary>
        /// Тмепература по цельсию
        /// </summary>
        public int TemperatureC { get; } = data.TemperatureC;

        /// <summary>
        /// Описание.
        /// </summary>
        public string? Summary { get; } = data.Summary;
    }
}
