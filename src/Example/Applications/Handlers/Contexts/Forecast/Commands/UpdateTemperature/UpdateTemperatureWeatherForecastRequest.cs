using Meta.Common.Contracts.Abstract;

namespace Meta.Example.Applications.Handlers.Contexts.Forecast.Commands.UpdateTemperature
{
    /// <summary>
    /// Запрос на обновленее температуры прогноза в конкретную дату.
    /// </summary>
    /// <param name="date">Дата.</param>
    /// <param name="temperatureC">Температура в цельсиях.</param>
    public class UpdateTemperatureWeatherForecastRequest(DateOnly date, int temperatureC) : Command
    {
        /// <summary>
        /// Дата.
        /// </summary>
        public DateOnly Date { get; } = date;

        /// <summary>
        /// Температура в цельсиях.
        /// </summary>
        public int TemperatureC { get; } = temperatureC;
    }
}
