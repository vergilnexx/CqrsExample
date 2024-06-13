using Meta.Common.Clients.Options;

namespace Meta.Example.Clients.Options
{
    /// <summary>
    /// Настройки клиента предсказания погоды.
    /// </summary>
    public class WeatherForecastClientOptions : IClientOptions
    {
        /// <summary>
        /// Название секции в конфиге.
        /// </summary>
        public static readonly string SectionName = "WeatherForecastClient";

        /// <inheritdoc/>
        public string Url { get; set; } = string.Empty;
    }
}
