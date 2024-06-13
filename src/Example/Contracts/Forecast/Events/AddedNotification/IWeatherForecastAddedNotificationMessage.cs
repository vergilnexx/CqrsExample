using Meta.Common.Contracts.Abstract;

namespace Meta.Example.Contracts.Forecast.Events.AddedNotification
{
    /// <summary>
    /// Сообщение об уведомлении добавления прогноза.
    /// </summary>
    public interface IWeatherForecastAddedNotificationMessage : IEvent
    {
        /// <summary>
        /// Дата.
        /// </summary>
        public DateOnly Date { get; set; }
    }
}
