using Meta.Common.Applications.AppServices.Contexts.Common.Services.DateTimeProvider;
using Meta.Common.Infrastructures.DataAccess.Repositories;
using Meta.Common.Test.Infrastructure;
using Meta.Example.Applications.AppServices.Contexts.Forecast.Services;
using Meta.Example.Clients.WeatherForecast;
using Meta.Example.Domain;
using Meta.Example.Infrastructures.DataAccess.Common.Configurators;
using Meta.Example.Infrastructures.DataAccess.Contexts;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Meta.Example.Tests.UnitTests.AppServices.Forecast.Services
{
    /// <summary>
    /// Тесты сервиса предсказания.
    /// </summary>
    internal class ForecastServiceTests : BaseMemoryDatabaseTests<ExampleDbContext, ExampleDbContextConfigurator>
    {
        [SetUp]
        public void Setup()
        {
            Init();
        }

        [TearDown]
        public void TearDown()
        {
            Cleanup();
        }

        /// <remarks>Паттерн AAA (Arrange, Act, Assert)</remarks>
        /// <remarks>Название теста: (название тестируемого метода)_(входные данные)_(ожидаемый результат)</remarks>
        [Test(Description = "Ответ не должен быть неопределенным")]
        public async Task GetAsync_RepositoryIsEmpty_ResultIsNotNull()
        {
            // Arrange
            var dateTimeProvider = new Mock<IDateTimeProvider>();
            var repository = _serviceProvider.GetRequiredService<IRepository<WeatherForecast>>();
            var weatherForecastServiceClient = new Mock<IWeatherForecastServiceClient>();

            var service = new ForecastService(dateTimeProvider.Object, repository, weatherForecastServiceClient.Object);

            // Act
            var result = await service.GetAsync(CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        /// <remarks>Паттерн AAA (Arrange, Act, Assert)</remarks>
        /// <remarks>Название теста: (название тестируемого метода)_(входные данные)_(ожидаемый результат)</remarks>
        [Test(Description = "Записей нет - ответ должен быть пустым")]
        public async Task GetAsync_RepositoryIsEmpty_ResultIsEmpty()
        {
            // Arrange
            var dateTimeProvider = new Mock<IDateTimeProvider>();
            var weatherForecastServiceClient = new Mock<IWeatherForecastServiceClient>();
            var repository = _serviceProvider.GetRequiredService<IRepository<WeatherForecast>>();

            var service = new ForecastService(dateTimeProvider.Object, repository, weatherForecastServiceClient.Object);

            // Act
            var result = await service.GetAsync(CancellationToken.None);

            // Assert
            Assert.That(result, Is.Empty);
        }

        /// <remarks>Паттерн AAA (Arrange, Act, Assert)</remarks>
        /// <remarks>Название теста: (название тестируемого метода)_(входные данные)_(ожидаемый результат)</remarks>
        [Test(Description = "В репозитории есть запись - в ответе должна быть эта запись")]
        public async Task GetAsync_RepositoryIsNotEmpty_ResultIsNotEmpty()
        {
            // Arrange
            var dateTimeProvider = new Mock<IDateTimeProvider>();
            var repository = _serviceProvider.GetRequiredService<IRepository<WeatherForecast>>();
            var date = DateOnly.FromDateTime(new DateTime(0, DateTimeKind.Utc).AddDays(1));
            var weatherForecastServiceClient = new Mock<IWeatherForecastServiceClient>();
            await repository.AddAsync(new WeatherForecast() { Date = date } , CancellationToken.None);

            var service = new ForecastService(dateTimeProvider.Object, repository, weatherForecastServiceClient.Object);

            // Act
            var result = await service.GetAsync(CancellationToken.None);

            // Assert
            Assert.That(result, Has.Count.EqualTo(1));
        }
    }
}
