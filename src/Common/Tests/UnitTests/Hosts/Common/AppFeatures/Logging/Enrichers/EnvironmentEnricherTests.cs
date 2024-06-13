using Meta.Common.Hosts.Features.AppFeatures.Logging.Enrichers;
using Meta.Common.Test.UnitTests.Hosts.Common.AppFeatures.Logging.Enrichers.Stubs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Moq;
using Serilog.Events;
using System.Net;
using System.Reflection;

namespace Meta.Common.Test.UnitTests.Hosts.Common.AppFeatures.Logging.Enrichers
{
    /// <summary>
    /// Тесты на обогащение логов данными окружения.
    /// </summary>
    internal class EnvironmentEnricherTests
    {
        [Test(Description = "При обогащении лога данными, должно быть больше добавлено больше одного свойства")]
        public void Enrich_Valid_PropertiesIsNotEmpty()
        {
            // Arrange
            var keys = new Dictionary<string, string?>();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var environment = new Mock<IHostEnvironment>();
            var environmentEnricher = new EnvironmentEnricher(configuration, environment.Object);
            var propertyFactory = new LogEventPropertyFactoryStub();
            var logEvent = new LogEvent(
                            new DateTimeOffset(0, TimeSpan.Zero),
                            LogEventLevel.Error,
                            exception: null,
                            MessageTemplate.Empty,
                            Array.Empty<LogEventProperty>());

            // Act
            environmentEnricher.Enrich(logEvent, propertyFactory);

            // Assert
            Assert.That(logEvent.Properties, Has.Count.Not.EqualTo(0));
        }

        [Test(Description = "При обогащении лога данными, должно быть задано название приложения")]
        public void Enrich_Valid_ApplicationNameIsNotNull()
        {
            // Arrange
            var keys = new Dictionary<string, string?>();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var environment = new Mock<IHostEnvironment>();
            var environmentEnricher = new EnvironmentEnricher(configuration, environment.Object);
            var propertyFactory = new LogEventPropertyFactoryStub();
            var logEvent = new LogEvent(
                            new DateTimeOffset(0, TimeSpan.Zero),
                            LogEventLevel.Error,
                            exception: null,
                            MessageTemplate.Empty,
                            Array.Empty<LogEventProperty>());

            // Act
            environmentEnricher.Enrich(logEvent, propertyFactory);

            // Assert
            var property = logEvent.Properties.FirstOrDefault(p => p.Key == "ApplicationName");
            Assert.That(property.Value?.ToString(), Is.EqualTo(Assembly.GetEntryAssembly()?.GetName().Name));
        }

        [Test(Description = "При обогащении лога данными, должно быть задано окружение приложения")]
        public void Enrich_Valid_EnvironmentNameIsNotNull()
        {
            // Arrange
            var keys = new Dictionary<string, string?>();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var environment = new Mock<IHostEnvironment>();
            var environmentEnricher = new EnvironmentEnricher(configuration, environment.Object);
            var propertyFactory = new LogEventPropertyFactoryStub();
            var logEvent = new LogEvent(
                            new DateTimeOffset(0, TimeSpan.Zero),
                            LogEventLevel.Error,
                            exception: null,
                            MessageTemplate.Empty,
                            Array.Empty<LogEventProperty>());

            // Act
            environmentEnricher.Enrich(logEvent, propertyFactory);

            // Assert
            var property = logEvent.Properties.FirstOrDefault(p => p.Key == "EnvironmentName");
            Assert.That(property.Value?.ToString(), Is.EqualTo("Unknown"));
        }

        [Test(Description = "При обогащении лога данными, должна быть заполнена версия приложения")]
        public void Enrich_Valid_ApplicationVersionIsNotNull()
        {
            // Arrange
            var keys = new Dictionary<string, string?>();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var environment = new Mock<IHostEnvironment>();
            var environmentEnricher = new EnvironmentEnricher(configuration, environment.Object);
            var propertyFactory = new LogEventPropertyFactoryStub();
            var logEvent = new LogEvent(
                            new DateTimeOffset(0, TimeSpan.Zero),
                            LogEventLevel.Error,
                            exception: null,
                            MessageTemplate.Empty,
                            Array.Empty<LogEventProperty>());

            // Act
            environmentEnricher.Enrich(logEvent, propertyFactory);

            // Assert
            var property = logEvent.Properties.FirstOrDefault(p => p.Key == "ApplicationVersion");
            Assert.That(property.Value?.ToString(), Is.EqualTo(Assembly.GetEntryAssembly()?.GetName()?.Version?.ToString()));
        }

        [Test(Description = "При обогащении лога данными, должно быть заполнено наименование машины")]
        public void Enrich_Valid_MachineNameIsNotNull()
        {
            // Arrange
            var keys = new Dictionary<string, string?>();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var environment = new Mock<IHostEnvironment>();
            var environmentEnricher = new EnvironmentEnricher(configuration, environment.Object);
            var propertyFactory = new LogEventPropertyFactoryStub();
            var logEvent = new LogEvent(
                            new DateTimeOffset(0, TimeSpan.Zero),
                            LogEventLevel.Error,
                            exception: null,
                            MessageTemplate.Empty,
                            Array.Empty<LogEventProperty>());

            // Act
            environmentEnricher.Enrich(logEvent, propertyFactory);

            // Assert
            var property = logEvent.Properties.FirstOrDefault(p => p.Key == "MachineName");
            Assert.That(property.Value?.ToString(), Is.EqualTo(Environment.MachineName));
        }

        [Test(Description = "При обогащении лога данными, должно быть заполнено наименование хоста")]
        public void Enrich_Valid_HostNameIsNotNull()
        {
            // Arrange
            var keys = new Dictionary<string, string?>();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var environment = new Mock<IHostEnvironment>();
            var environmentEnricher = new EnvironmentEnricher(configuration, environment.Object);
            var propertyFactory = new LogEventPropertyFactoryStub();
            var logEvent = new LogEvent(
                            new DateTimeOffset(0, TimeSpan.Zero),
                            LogEventLevel.Error,
                            exception: null,
                            MessageTemplate.Empty,
                            Array.Empty<LogEventProperty>());

            // Act
            environmentEnricher.Enrich(logEvent, propertyFactory);

            // Assert
            var property = logEvent.Properties.FirstOrDefault(p => p.Key == "HostName");
            Assert.That(property.Value?.ToString(), Is.EqualTo(Dns.GetHostName()));
        }

        [Test(Description = "При обогащении лога данными, должно быть заполнено наименование стенда")]
        public void Enrich_Valid_StandNameIsNotNull()
        {
            // Arrange
            var keys = new Dictionary<string, string?>();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var environment = new Mock<IHostEnvironment>();
            var environmentEnricher = new EnvironmentEnricher(configuration, environment.Object);
            var propertyFactory = new LogEventPropertyFactoryStub();
            var logEvent = new LogEvent(
                            new DateTimeOffset(0, TimeSpan.Zero),
                            LogEventLevel.Error,
                            exception: null,
                            MessageTemplate.Empty,
                            Array.Empty<LogEventProperty>());

            // Act
            environmentEnricher.Enrich(logEvent, propertyFactory);

            // Assert
            var property = logEvent.Properties.FirstOrDefault(p => p.Key == "StandName");
            Assert.That(property.Value?.ToString(), Is.EqualTo("Unknown"));
        }
    }
}
