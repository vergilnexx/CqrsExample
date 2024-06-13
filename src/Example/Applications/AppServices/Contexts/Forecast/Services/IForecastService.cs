using Meta.Example.Applications.AppServices.Contexts.Forecast.Models;
using Meta.Example.Contracts.Forecast.Response;

namespace Meta.Example.Applications.AppServices.Contexts.Forecast.Services
{
    /// <summary>
    /// Сервис для работы с предсказаниями.
    /// </summary>
    public interface IForecastService
    {
        /// <summary>
        /// Возвращает данные о предсказаниях.
        /// </summary>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Данные о предсказаниях.</returns>
        Task<IReadOnlyCollection<WeatherForecastDto>> GetAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Возвращает данные о предсказаниях.
        /// </summary>
        /// <param name="date">Дата.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Данные о предсказаниях.</returns>
        Task<WeatherForecastDto?> FindAsync(DateOnly date, CancellationToken cancellationToken);

        /// <summary>
        /// Добавляет прогноз погоды.
        /// </summary>
        /// <param name="model">Данные</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Идентификатор созданной записи.</returns>
        Task<int> AddAsync(AddWeatherForecastModel model, CancellationToken cancellationToken);

        /// <summary>
        /// Обновляет температуру прогноза в конкретную дату.
        /// </summary>
        /// <param name="date">Дата.</param>
        /// <param name="temperatureC">Дата.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        Task UpdateTemperatureAsync(DateOnly date, int temperatureC, CancellationToken cancellationToken);
        
        /// <summary>
        /// Удаление прогноза.
        /// </summary>
        /// <param name="date">Дата.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        Task DeleteAsync(DateOnly date, CancellationToken cancellationToken);

        /// <summary>
        /// Возвращает признак, что прогноз удален.
        /// </summary>
        /// <param name="date">Дата.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Признак, что прогноз удален.</returns>
        Task<bool> IsDeletedAsync(DateOnly date, CancellationToken cancellationToken);
    }
}
