namespace Meta.Example.Contracts.Forecast.Response
{
    /// <summary>
    /// Данные о прогнозе погоды.
    /// </summary>
    public class WeatherForecastDto
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
        /// Температура по фаренгейту
        /// </summary>
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        /// <summary>
        /// Описание.
        /// </summary>
        public string? Summary { get; set; }
    }
}
