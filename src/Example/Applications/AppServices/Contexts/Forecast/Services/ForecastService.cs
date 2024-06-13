using Meta.Common.Applications.AppServices.Contexts.Common.Services.DateTimeProvider;
using Meta.Common.Contracts.Exceptions.Common;
using Meta.Common.Infrastructures.DataAccess.Repositories;
using Meta.Example.Applications.AppServices.Contexts.Forecast.Models;
using Meta.Example.Clients.WeatherForecast;
using Meta.Example.Contracts.Forecast.Response;
using Meta.Example.Domain;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Meta.Example.Applications.AppServices.Contexts.Forecast.Services
{
    /// <inheritdoc/>
    public class ForecastService(
        IDateTimeProvider _dateTimeProvider,
        IRepository<WeatherForecast> _repository,
        IWeatherForecastServiceClient _weatherForecastServiceClient) : IForecastService
    {
        /// <inheritdoc/>
        public async Task<IReadOnlyCollection<WeatherForecastDto>> GetAsync(CancellationToken cancellationToken)
        {
            var dates = Enumerable.Range(1, 5)
                                  .Select(index => DateOnly.FromDateTime(_dateTimeProvider.UtcNow.AddDays(index)));

            var forecasts = await  IsActive()
                                    .Where(wf => dates.Contains(wf.Date))
                                    .Select(wf => new WeatherForecastDto
                                    {
                                        Date = wf.Date,
                                        TemperatureC = wf.TemperatureC,
                                        Summary = wf.Summary
                                    })
                                    .ToArrayAsync(cancellationToken);
            return forecasts;
        }

        /// <inheritdoc/>
        public async Task<WeatherForecastDto?> FindAsync(DateOnly date, CancellationToken cancellationToken)
        {
            var forecast = await FilteredByDate(date)
                                    .Select(wf => new WeatherForecastDto
                                    {
                                        Date = wf.Date,
                                        TemperatureC = wf.TemperatureC,
                                        Summary = wf.Summary
                                    })
                                    .FirstOrDefaultAsync(cancellationToken);
            return forecast;
        }

        /// <inheritdoc/>
        public async Task<int> AddAsync(AddWeatherForecastModel model, CancellationToken cancellationToken)
        {
            var isExist = await FilteredByDate(model.Date).AnyAsync(cancellationToken);
            if (isExist)
            {
                await _weatherForecastServiceClient.DeleteAsync(model.Date, cancellationToken);
            }

            var entity = new WeatherForecast();
            entity.Date = model.Date;
            entity.TemperatureC = model.TemperatureC;
            entity.Summary = model.Summary;

            await _repository.AddAsync(entity, cancellationToken);
            return entity.Id;
        }

        /// <inheritdoc/>
        public async Task UpdateTemperatureAsync(DateOnly date, int temperatureC, CancellationToken cancellationToken)
        {
            var entity = await FilteredByDate(date).FirstOrDefaultAsync(cancellationToken)
                                ?? throw new NotFoundException("Не удалось найти прогноз на дату: " + date.ToString());

            entity.TemperatureC = temperatureC;

            await _repository.UpdateAsync(entity, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(DateOnly date, CancellationToken cancellationToken)
        {
            var entity = await FilteredByDate(date).FirstOrDefaultAsync(cancellationToken)
                                ?? throw new NotFoundException("Не удалось найти прогноз на дату: " + date.ToString());

            entity.IsDeleted = true;

            await _repository.UpdateAsync(entity, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<bool> IsDeletedAsync(DateOnly date, CancellationToken cancellationToken)
        {
            return await _repository.Where(r => r.Date == date).AnyAsync(cancellationToken);
        }

        private IQueryable<WeatherForecast> IsActive()
        {
            return _repository.Where(r => r.IsDeleted == false);
        }

        private IQueryable<WeatherForecast> FilteredByDate(DateOnly date)
        {
            return IsActive().Where(r => r.Date == date);
        }
    }
}
