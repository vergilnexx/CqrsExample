namespace Meta.Example.Clients.WeatherForecast
{
    /// <summary>
    /// Клиент для работы с сервисом предсказания погоды.
    /// </summary>
    public interface IWeatherForecastServiceClient
    {
        /// <summary>
        /// Удаляет прогноз на конкретную дату.
        /// </summary>
        /// <param name="date">Дата.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        public Task DeleteAsync(DateOnly date, CancellationToken cancellationToken);
    }
}
