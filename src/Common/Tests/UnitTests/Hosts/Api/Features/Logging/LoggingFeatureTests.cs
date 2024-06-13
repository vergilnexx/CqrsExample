using Meta.Common.Hosts.Api.Features.AppFeatures.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;

namespace Meta.Common.Tests.UnitTests.Hosts.Api.Features.Logging
{
    /// <summary>
    /// Тесты функциональности логирования.
    /// </summary>
    internal class LoggingFeatureTests
    {
        [Test(Description = "Добавление функциональности логирования должно проходить без исключений.")]
        public void AddFeature_Register_LoggerIsNotNull() 
        {
            // Arrange
            var feature = new LoggingFeature();
            var services = new ServiceCollection();
            var hostBuilder = new Mock<IHostBuilder>();
            var loggingBuilder = new Mock<ILoggingBuilder>();

            // Act
            // Assert
            Assert.DoesNotThrow(() =>
                feature.AddFeature(services, hostBuilder.Object, loggingBuilder.Object));
        }

        [Test(Description = "Конфигурирование функциональности логирования должно проходить без исключений.")]
        public void Configure_Register_LoggerIsNotNull()
        {
            // Arrange
            var context = new HostBuilderContext(new Dictionary<object, object>());
            var keys = new Dictionary<string, string?>();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            context.Configuration = configuration;
            var logger = new LoggerConfiguration();

            // Act
            // Assert
            Assert.DoesNotThrow(() =>
                LoggingFeature.Configure(context, logger));
        }
    }
}
