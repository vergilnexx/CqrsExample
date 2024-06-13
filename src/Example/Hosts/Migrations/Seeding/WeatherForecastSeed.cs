using Meta.Example.Domain;
using Meta.Example.Infrastructures.DataAccess.Contexts;

namespace Meta.Example.Migrations.Seeding
{
    /// <summary>
    /// Seed'ы предсказаний погоды.
    /// </summary>
    internal class WeatherForecastSeed : ISeed
    {
        /// <summary>
        /// Возвращает предсказания.
        /// </summary>
        /// <returns>Предсказания.</returns>
        internal static IReadOnlyCollection<WeatherForecast> Get() =>
            [
                new WeatherForecast() { Id = 1, Date = new DateOnly(2024, 03, 08), Summary = "Холодно", TemperatureC = -10 },
                new WeatherForecast() { Id = 2, Date = new DateOnly(2024, 03, 09), Summary = "Около нуля", TemperatureC = 0 },
                new WeatherForecast() { Id = 3, Date = new DateOnly(2024, 03, 10), Summary = "Солнечно", TemperatureC = 20 }
            ];

        /// <inheritdoc/>
        public async Task FillAsync(ExampleDbContext context, CancellationToken cancellationToken)
        {
            var forecasts = Get();
            foreach (var forecast in forecasts)
            {
                var entity = await context.FindAsync<WeatherForecast>(forecast.Id, cancellationToken);
                if (entity == null)
                {
                    await context.AddAsync(forecast, cancellationToken);
                }
                else
                {
                    entity.Date = forecast.Date;
                    entity.Summary = forecast.Summary;
                    entity.TemperatureC = forecast.TemperatureC;
                    entity.IsDeleted = forecast.IsDeleted;
                }
            }
        }
    }
}
