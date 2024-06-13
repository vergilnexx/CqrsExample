using Meta.Common.Hosts.Features.AppFeatures.Logging.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Moq;
using Serilog;
using Serilog.Settings.Configuration;
using LoggerExtensions = Meta.Common.Hosts.Features.AppFeatures.Logging.Extensions.LoggerExtensions;

namespace Meta.Common.Test.UnitTests.Hosts.Common.AppFeatures.Logging.Extensions
{
    internal class LoggerExtensionsTests
    {
        [Test(Description = "Если настройки обогащения не переданы, то выбрасывается исключение.")]
        public void WithEnvironment_EnrichmentConfigurationIsNull_ArgumentNullException()
        {
            // Arrange
            var keys = new Dictionary<string, string?>();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var environment = new Mock<IHostEnvironment>();

            // Act
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                LoggerExtensions.WithEnvironment(enrichmentConfiguration: null, configuration, environment.Object);
            });

            // Assert
            Assert.That(exception?.ParamName, Is.EqualTo("enrichmentConfiguration"));
        }

        [Test(Description = "Если настройки не переданы, то выбрасывается исключение.")]
        public void WithEnvironment_ConfigurationIsNull_ArgumentNullException()
        {
            // Arrange
            var keys = new Dictionary<string, string?>();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var loggerConfiguration = new Mock<LoggerConfiguration>();
            var environment = new Mock<IHostEnvironment>();
            var options = new ConfigurationReaderOptions { SectionName = "Features:Logging:Serilog" };

            // Act
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                loggerConfiguration.Object
                        .ReadFrom.Configuration(configuration, options)
                        .Enrich.WithEnvironment(configuration: null, environment.Object);
            });

            // Assert
            Assert.That(exception?.ParamName, Is.EqualTo("configuration"));
        }

        [Test(Description = "Если настройки не переданы, то выбрасывается исключение.")]
        public void WithEnvironment_Valid_NoException()
        {
            // Arrange
            var keys = new Dictionary<string, string?>();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var loggerConfiguration = new Mock<LoggerConfiguration>();
            var environment = new Mock<IHostEnvironment>();
            var options = new ConfigurationReaderOptions { SectionName = "Features:Logging:Serilog" };

            // Act
            // Assert
            Assert.DoesNotThrow(() =>
            {
                loggerConfiguration.Object
                        .ReadFrom.Configuration(configuration, options)
                        .Enrich.WithEnvironment(configuration, environment.Object);
            });
        }
    }
}
