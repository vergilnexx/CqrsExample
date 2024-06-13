using Meta.Common.Contracts.Abstract;

namespace Meta.Example.Contracts.Forecast.Events.Delete
{
    /// <inheritdoc/>
    public class DeleteWeatherForecastMessage : Event, IDeleteWeatherForecastMessage
    {
        /// <inheritdoc/>
        public required DateOnly Date { get; set; }
    }
}
