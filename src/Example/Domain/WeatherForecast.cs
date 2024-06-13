using Meta.Common.Domain;

namespace Meta.Example.Domain
{
    /// <summary>
    /// Данные о прогнозе погоды.
    /// </summary>
    public class WeatherForecast : Entity<int>
    {
        /// <summary>
        /// Дата.
        /// </summary>
        public DateOnly Date { get; set; }

        /// <summary>
        /// Температура по цельсию.
        /// </summary>
        public int TemperatureC { get; set; }

        /// <summary>
        /// Описание.
        /// </summary>
        public string? Summary { get; set; }

        /// <summary>
        /// Признак, что прогноз удален.
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
