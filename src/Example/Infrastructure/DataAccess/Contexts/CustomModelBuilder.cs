using Meta.Common.Infrastructures.DataAccess.Configurations;
using Meta.Example.Infrastructures.DataAccess.Contexts.Forecast.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Meta.Example.Infrastructures.DataAccess.Contexts
{
    /// <summary>
    /// Сборщик моделей.
    /// </summary>
    public static class CustomModelBuilder
    {
        /// <summary>
        /// Создает модель.
        /// </summary>
        /// <param name="modelBuilder">Билдер.</param>
        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureForecastModels(modelBuilder);

            ModelBuilderExtension.SetDefaultDateTimeKind(modelBuilder, DateTimeKind.Utc);
        }

        private static void ConfigureForecastModels(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new WeatherForecastConfiguration());
        }
    }
}
