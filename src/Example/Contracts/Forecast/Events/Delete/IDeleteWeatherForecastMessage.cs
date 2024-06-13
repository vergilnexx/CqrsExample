using Meta.Common.Contracts.Abstract;

namespace Meta.Example.Contracts.Forecast.Events.Delete
{
    /// <summary>
    /// Сообщение о необходимости удалить прогноз погоды.
    /// </summary>
    public interface IDeleteWeatherForecastMessage : IEvent
    {
        /// <summary>
        /// Дата прогноза.
        /// </summary>
        public DateOnly Date { get; set; }
    }
}
