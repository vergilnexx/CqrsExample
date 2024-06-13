namespace Meta.Example.Applications.AppServices.Contexts.Forecast.Models
{
    /// <summary>
    /// Модель для добавления прогноза погоды.
    /// </summary>
    public class AddWeatherForecastModel
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="date">Дата</param>
        /// <param name="temperatureC">Температура по цельсию</param>
        public AddWeatherForecastModel(DateOnly date, int temperatureC)
        {
            Date = date;
            TemperatureC = temperatureC;
        }

        /// <summary>
        /// Дата.
        /// </summary>
        public DateOnly Date { get; }

        /// <summary>
        /// Температура по цельсию
        /// </summary>
        public int TemperatureC { get; }

        /// <summary>
        /// Описание.
        /// </summary>
        public string? Summary { get; set; }
    }
}
