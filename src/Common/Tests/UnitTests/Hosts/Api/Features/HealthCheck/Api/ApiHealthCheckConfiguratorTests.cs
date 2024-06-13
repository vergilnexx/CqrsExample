using Meta.Common.Hosts.Api.Features.AppFeatures.HealthCheck.Instances.Api;
using Meta.Common.Hosts.Features.AppFeatures.HealthCheck.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;

namespace Meta.Common.Tests.UnitTests.Hosts.Api.Features.HealthCheck.Api
{
    /// <summary>
    /// Тесты конфигуратора health check'ов API.
    /// </summary>
    internal class ApiHealthCheckConfiguratorTests
    {
        [Test(Description = "Если настройки не заданы, то не вызывается создание проверки функциональности")]
        public void Configure_OptionSectionIsEmpty_AddCheckIsNotCalled()
        {
            // Arrange
            var services = new Mock<IServiceCollection>();
            var keys = new Dictionary<string, string?>()
            {
                { "HealthCheck", string.Empty },
                { "HealthCheck:Sections", string.Empty },
                { "HealthCheck:Sections:0", string.Empty },
                { "HealthCheck:Sections:0:Name", "Api" }
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var checksBuilder = new Mock<IHealthChecksBuilder>();
            var configurator = new ApiHealthCheckConfigurator();

            // Act
            configurator.Configure(services.Object, configuration, configuration.GetSection(""), checksBuilder.Object);

            // Assert
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
                { "HealthCheck:Sections:0:Name", "Api" },
                { "HealthCheck:Sections:0:Options", string.Empty },
                { "HealthCheck:Sections:0:Options:Name", "Api" },
                { "HealthCheck:Sections:0:Options:FailureStatus", string.Empty },
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var checksBuilder = new Mock<IHealthChecksBuilder>();
            var configurator = new ApiHealthCheckConfigurator();

            // Act
            var exception = Assert.Throws<HealthCheckConfigurationException>(
                () => configurator.Configure(services.Object, configuration, configuration.GetSection("HealthCheck:Sections:0"), checksBuilder.Object));

            // Assert
            Assert.That(exception, Is.Not.Null);
        }

        [Test(Description = "Если настройки валидны, то вызывается создание проверки функциональности")]
        public void Configure_ValidOptions_AddCheckWillBeCalledOnce()
        {
            // Arrange
            var services = new Mock<IServiceCollection>();
            var keys = new Dictionary<string, string?>()
            {
                { "HealthCheck", string.Empty },
                { "HealthCheck:Sections", string.Empty },
                { "HealthCheck:Sections:0", string.Empty },
                { "HealthCheck:Sections:0:Name", "Api" },
                { "HealthCheck:Sections:0:Options", string.Empty },
                { "HealthCheck:Sections:0:Options:Name", "Api" },
                { "HealthCheck:Sections:0:Options:FailureStatus", "Unhealthy" },
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var checksBuilder = new Mock<IHealthChecksBuilder>();
            var configurator = new ApiHealthCheckConfigurator();

            // Act
            configurator.Configure(services.Object, configuration, configuration.GetSection("HealthCheck:Sections:0"), checksBuilder.Object);

            // Assert
            checksBuilder.Verify(b => b.Add(It.IsAny<HealthCheckRegistration>()), Times.Once);
        }
    }
}
