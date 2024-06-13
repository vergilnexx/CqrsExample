using Meta.Common.Hosts.Api.Features.AppFeatures.HealthCheck.Instances.RabbitMq;
using Meta.Common.Hosts.Features.AppFeatures.HealthCheck.Base;
using Meta.Common.Hosts.Features.AppFeatures.HealthCheck.Instances.RabbitMq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;

namespace Meta.Common.Tests.UnitTests.Hosts.Api.Features.HealthCheck.RabbitMq
{
    /// <summary>
    /// Тесты проверки функциональности RabbitMq.
    /// </summary>
    internal class RabbitMqHealthCheckConfiguratorTests
    {
        [Test(Description = "Если настройки не заданы, то не вызывается создание проверки функциональности")]
        public void Configure_OptionSectionIsEmpty_AddCheckIsNotCalled()
        {
            // Assert
            var services = new Mock<IServiceCollection>();
            var keys = new Dictionary<string, string?>()
            {
                { "HealthCheck", string.Empty },
                { "HealthCheck:Sections", string.Empty },
                { "HealthCheck:Sections:0", string.Empty },
                { "HealthCheck:Sections:0:Name", "RabbitMq" }
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var checksBuilder = new Mock<IHealthChecksBuilder>();
            var configurator = new RabbitMqHealthCheckConfigurator();

            // Act
            configurator.Configure(services.Object, configuration, configuration.GetSection(""), checksBuilder.Object);

            // Arrange
            checksBuilder.Verify(b => b.Add(It.IsAny<HealthCheckRegistration>()), Times.Never);
        }

        [Test(Description = "Если статус в настройках не задан, то выбрасывается исключение")]
        public void Configure_FailureStatusIsNotParsed_HealthCheckConfigurationException()
        {
            // Arrange
            var services = new Mock<IServiceCollection>();
            var keys = new Dictionary<string, string?>()
            {
                { "HealthCheck", string.Empty },
                { "HealthCheck:Sections", string.Empty },
                { "HealthCheck:Sections:0", string.Empty },
                { "HealthCheck:Sections:0:Name", "RabbitMq" },
                { "HealthCheck:Sections:0:Options", string.Empty },
                { "HealthCheck:Sections:0:Options:Name", "RabbitMq" },
                { "HealthCheck:Sections:0:Options:FailureStatus", string.Empty },
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var checksBuilder = new Mock<IHealthChecksBuilder>();
            var configurator = new RabbitMqHealthCheckConfigurator();

            // Act
            var exception = Assert.Throws<HealthCheckConfigurationException>(
                () => configurator.Configure(services.Object, configuration, configuration.GetSection("HealthCheck:Sections:0"), checksBuilder.Object));

            // Assert
            Assert.That(exception, Is.Not.Null);
        }

        [Test(Description = "Если timeout в настройках не задан, то выбрасывается исключение")]
        public void Configure_TimeoutIsEmpty_HealthCheckConfigurationException()
        {
            // Arrange
            var services = new Mock<IServiceCollection>();
            var keys = new Dictionary<string, string?>()
            {
                { "HealthCheck", string.Empty },
                { "HealthCheck:Sections", string.Empty },
                { "HealthCheck:Sections:0", string.Empty },
                { "HealthCheck:Sections:0:Name", "RabbitMq" },
                { "HealthCheck:Sections:0:Options", string.Empty },
                { "HealthCheck:Sections:0:Options:Name", "RabbitMq" },
                { "HealthCheck:Sections:0:Options:Timeout", string.Empty }
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var checksBuilder = new Mock<IHealthChecksBuilder>();
            var configurator = new RabbitMqHealthCheckConfigurator();

            // Act
            var exception = Assert.Throws<HealthCheckConfigurationException>(
                () => configurator.Configure(services.Object, configuration, configuration.GetSection("HealthCheck:Sections:0"), checksBuilder.Object));

            // Assert
            Assert.That(exception, Is.Not.Null);
        }

        [Test(Description = "Если connectionString в настройках не задан, то выбрасывается исключение")]
        public void Configure_ConnectionStringsIsEmpty_HealthCheckConfigurationException()
        {
            // Arrange
            var services = new Mock<IServiceCollection>();
            var keys = new Dictionary<string, string?>()
            {
                { "HealthCheck", string.Empty },
                { "HealthCheck:Sections", string.Empty },
                { "HealthCheck:Sections:0", string.Empty },
                { "HealthCheck:Sections:0:Name", "RabbitMq" },
                { "HealthCheck:Sections:0:Options", string.Empty },
                { "HealthCheck:Sections:0:Options:Name", "RabbitMq" },
                { "HealthCheck:Sections:0:Options:ConfigSection", "db" },
                { "db", string.Empty },
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var checksBuilder = new Mock<IHealthChecksBuilder>();
            var configurator = new RabbitMqHealthCheckConfigurator();

            // Act
            var exception = Assert.Throws<HealthCheckConfigurationException>(
                () => configurator.Configure(services.Object, configuration, configuration.GetSection("HealthCheck:Sections:0"), checksBuilder.Object));

            // Assert
            Assert.That(exception, Is.Not.Null);
        }

        [Test(Description = "Если при настройке проблемы связи, то выбрасывается исключение")]
        public void Configure_ConnectionError_HealthCheckConfigurationException()
        {
            // Arrange
            var services = new Mock<IServiceCollection>();
            var keys = new Dictionary<string, string?>()
            {
                { "HealthCheck", string.Empty },
                { "HealthCheck:Sections", string.Empty },
                { "HealthCheck:Sections:0", string.Empty },
                { "HealthCheck:Sections:0:Name", "RabbitMq" },
                { "HealthCheck:Sections:0:Options", string.Empty },
                { "HealthCheck:Sections:0:Options:Name", "RabbitMq" },
                { "HealthCheck:Sections:0:Options:FailureStatus", "Unhealthy" },
                { "HealthCheck:Sections:0:Options:ConfigSection", "db" },
                { "db", string.Empty },
                { "db:Hosts", string.Empty },
                { "db:Hosts:0", "localhost" },
                { "db:VirtualHost", "localhost" },
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var checksBuilder = new Mock<IHealthChecksBuilder>();
            var configurator = new RabbitMqHealthCheckConfigurator();

            // Act
            var exception = Assert.Throws<HealthCheckConfigurationException>(
                () => configurator.Configure(services.Object, configuration, configuration.GetSection("HealthCheck:Sections:0"), checksBuilder.Object));

            // Assert
            Assert.That(exception, Is.Not.Null);
        }
    }
}
