using MediatR;
using Meta.Common.Applications.AppServices.Contexts.Common.Services.DateTimeProvider;
using Meta.Common.Contracts.Exceptions.Common;
using Meta.Common.Hosts.RabbitMq.Registrar.Client;
using Meta.Common.Infrastructures.DataAccess.Repositories;
using Meta.Common.Test.Infrastructure;
using Meta.Example.Contracts.Forecast.Requests;
using Meta.Example.Contracts.Forecast.Response;
using Meta.Example.Domain;
using Meta.Example.Infrastructures.DataAccess.Common.Configurators;
using Meta.Example.Infrastructures.DataAccess.Contexts;
using Meta.Example.Public.Hosts.Api;
using Meta.Public.Hosts.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Meta.Example.Tests.IntegrationTests.PublicApi
{
    /// <summary>
    /// Тесты контроллера <see cref="WeatherForecastController" />
    /// </summary>
    internal class WeatherForecastControllerTests : BaseMemoryDatabaseTests<ExampleDbContext, ExampleDbContextConfigurator>
    {
        private Mock<IDateTimeProvider> _dateTimeProvider;
        private Mock<IRabbitMqClient> _rabbitMqClient;

        [SetUp]
        public void Setup()
        {
            var keys = new Dictionary<string, string?>();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(keys)
                .Build();
            var currentAssembly = typeof(WeatherForecastController).Assembly;
            _services.AddScoped<IConfiguration>(_ => configuration);
            _dateTimeProvider = new Mock<IDateTimeProvider>();
            _rabbitMqClient = new Mock<IRabbitMqClient>();
            _services.AddScoped(_ => _dateTimeProvider.Object);
            _services.AddScoped(_ => _rabbitMqClient.Object);
            _services
                .AddHandlers(currentAssembly)
                .AddServices()
                .AddLogging(config => { config.AddDebug(); });

            Init();
        }

        [TearDown]
        public void TearDown()
        {
            Cleanup();
        }

        [Test(Description = "При вызове метода получения данных, ответ должен быть 200 ОК")]
        public async Task Get_EmptyData_200Ok()
        {
            // Arrange
            var mediatr = _serviceProvider.GetRequiredService<IMediator>();
            var controller = new WeatherForecastController(mediatr);

            // Act
            var result = await controller.Get(CancellationToken.None);

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test(Description = "При вызове метода получения данных, когда в репозитории есть одна запись, в ответе должна быть тоже одна запись")]
        public async Task Get_OneEntity_OneItem()
        {
            // Arrange
            var mediatr = _serviceProvider.GetRequiredService<IMediator>();
            var controller = new WeatherForecastController(mediatr);
            var repository = _serviceProvider.GetRequiredService<IRepository<WeatherForecast>>();
            await repository.AddAsync(new WeatherForecast() { Date = new DateOnly(2020, 1, 2) }, CancellationToken.None);
            _dateTimeProvider.SetupGet(p => p.UtcNow).Returns(new DateTime(2020, 1, 1));

            // Act
            var result = await controller.Get(CancellationToken.None);

            // Assert
            var okResult = result as OkObjectResult;
            var list = ((IReadOnlyCollection<WeatherForecastDto>?)okResult?.Value) ?? [];
            Assert.That(list, Has.Count.EqualTo(1));
        }

        [Test(Description = "При вызове метода получения данных, когда в репозитории есть записи, в ответе должны быть записи подходящие по датам")]
        public async Task Get_FilterEntityByDate_OneItem()
        {
            // Arrange
            var mediatr = _serviceProvider.GetRequiredService<IMediator>();
            var controller = new WeatherForecastController(mediatr);
            var repository = _serviceProvider.GetRequiredService<IRepository<WeatherForecast>>();
            await repository.AddAsync(new WeatherForecast() { Date = new DateOnly(2020, 1, 1) }, CancellationToken.None);
            await repository.AddAsync(new WeatherForecast() { Date = new DateOnly(2020, 1, 2) }, CancellationToken.None);
            _dateTimeProvider.SetupGet(p => p.UtcNow).Returns(new DateTime(2020, 1, 1));

            // Act
            var result = await controller.Get(CancellationToken.None);

            // Assert
            var okResult = result as OkObjectResult;
            var list = ((IReadOnlyCollection<WeatherForecastDto>?)okResult?.Value) ?? [];
            Assert.That(list, Has.Count.EqualTo(1));
            Assert.That(list.First().Date, Is.EqualTo(new DateOnly(2020, 1, 2)));
        }

        [Test(Description = "При вызове метода добавления данных, в репозитории должна появляться новая запись")]
        public async Task Add_OneItem_OneItem()
        {
            // Arrange
            var mediatr = _serviceProvider.GetRequiredService<IMediator>();
            var controller = new WeatherForecastController(mediatr);
            var repository = _serviceProvider.GetRequiredService<IRepository<WeatherForecast>>();

            // Act
            await controller.Add(new WeatherForecastData() { Date = new DateOnly(2020, 1, 2) }, CancellationToken.None);

            // Assert
            var count = await repository.AsQueryable().CountAsync(CancellationToken.None);
            Assert.That(count, Is.EqualTo(1));
        }

        [Test(Description = "При вызове метода добавления данных, должен возвращаться идентификатор")]
        public async Task Add_OneItem_ReturnId()
        {
            // Arrange
            var mediatr = _serviceProvider.GetRequiredService<IMediator>();
            var controller = new WeatherForecastController(mediatr);

            // Act
            var result = await controller.Add(new WeatherForecastData() { Date = new DateOnly(2020, 1, 2) }, CancellationToken.None);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That((int?)okResult?.Value, Is.EqualTo(1));
        }

        [Test(Description = "При вызове метода добавления данных, должен возвращаться идентификатор")]
        public async Task Add_AddExistedDate_Exception()
        {
            // Arrange
            var mediatr = _serviceProvider.GetRequiredService<IMediator>();
            var controller = new WeatherForecastController(mediatr);
            var repository = _serviceProvider.GetRequiredService<IRepository<WeatherForecast>>();
            var date = new DateOnly(2020, 1, 1);
            await repository.AddAsync(new WeatherForecast() { Date = date }, CancellationToken.None);

            // Act
            var exception = Assert.ThrowsAsync<ReadableException>(async () =>
                                await controller.Add(new WeatherForecastData() { Date = date }, CancellationToken.None));

            // Assert
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.EqualTo($"Предсказание погоды для даты '{date}' уже существует"));
        }
    }
}
