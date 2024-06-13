using Meta.Common.Contracts.Abstract;

namespace Meta.Example.Applications.Handlers.Contexts.Forecast.Commands.DeleteWeatherForecast
{
    /// <summary>
    /// Команда на удаление прогноза погоды.
    /// </summary>
    public class DeleteWeatherForecastRequest : Command
    {
        /// <summary>
        /// Дата прогноза.
        /// </summary>
        public DateOnly Date { get; }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="date">Дата прогноза.</param>
        public DeleteWeatherForecastRequest(DateOnly date)
        {
            Date = date;
        }
    }
}
