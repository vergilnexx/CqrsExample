using Meta.Common.Contracts.Abstract;

namespace Meta.Example.Contracts.Forecast.Events.AddedNotification
{
    /// <inheritdoc/>
    public class WeatherForecastAddedNotificationMessage : Event, IWeatherForecastAddedNotificationMessage
    {
        /// <inheritdoc/>
        public DateOnly Date { get; set; }
    }
}
