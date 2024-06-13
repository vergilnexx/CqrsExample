using Meta.Example.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Meta.Example.Infrastructures.DataAccess.Contexts.Forecast.Configurations
{
    /// <summary>
    /// Конфигурация сущности <see cref="WeatherForecast"/>
    /// </summary>
    public class WeatherForecastConfiguration : IEntityTypeConfiguration<WeatherForecast>
    {
        /// <inheritdoc/>
        public void Configure(EntityTypeBuilder<WeatherForecast> builder)
        {
            builder.HasKey(wf => wf.Id);

            builder.Property(x => x.Date).IsRequired();
            builder.Property(x => x.TemperatureC).IsRequired();
            builder.Property(x => x.Summary).HasMaxLength(2000).IsUnicode();
            builder.Property(x => x.IsDeleted).IsRequired();
        }
    }
}
