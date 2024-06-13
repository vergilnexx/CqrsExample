namespace Meta.Example.Contracts.Forecast.Requests
{
    /// <summary>
    /// Данные прогноза погоды.
    /// </summary>
    public class WeatherForecastData
    {
        /// <summary>
        /// Дата.
        /// </summary>
        public DateOnly Date { get; set; }

        /// <summary>
        /// Тмепература по цельсию
        /// </summary>
        public int TemperatureC { get; set; }

        /// <summary>
        /// Описание.
        /// </summary>
        public string? Summary { get; set; }
    }
}
