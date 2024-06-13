using Meta.Common.Hosts.Features.AppFeatures.Base;
using Meta.Common.Hosts.Features.AppFeatures.HealthCheck;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Reflection;

namespace Meta.Common.Test.UnitTests.Hosts.Common.AppFeatures.Base
{
    /// <summary>
    /// Тест фабрики регистрации функциональности.
    /// </summary>
    internal class AppFeatureFactoryTests
    {
        [Test(Description = "Если в конфигурации нет секции, то функциональностей тоже нет")]
        public void AppFeature_ConfigurationIsEmpty_AppFeaturesIsEmpty()
        {
            // Arrange
            var configuration = new Mock<IConfiguration>();
            var assembly = new Mock<Assembly>();

            // Act
            var services = AppFeatureFactory.GetAppFeatures(configuration.Object, [assembly.Object]);

            // Assert
            Assert.That(services, Is.Empty);
        }

        [Test(Description = "Если в секции конфигураци не определены функциональности, то должен быть пустой список")]
        public void AppFeature_FeatureConfigurationSectionIsEmpty_AppFeaturesIsEmpty()
        {
            // Arrange
            var configuration = new Mock<IConfiguration>();
            var featureConfigurationSection = new Mock<IConfigurationSection>();
            configuration.Setup(c => c.GetSection("Features")).Returns(featureConfigurationSection.Object);
            var assembly = new Mock<Assembly>();

            // Act
            var services = AppFeatureFactory.GetAppFeatures(configuration.Object, [assembly.Object]);

            // Assert
            Assert.That(services, Is.Empty);
        }

        [Test(Description = "Если в секции конфигураци функциональность отключена, то должен быть пустой список")]
        public void AppFeature_FeatureDisabled_AppFeaturesIsEmpty()
        {
            // Arrange
            var keys = new Dictionary<string, string?>
            {
               { "Features", string.Empty },
               { "Features:Logging", string.Empty },
               { "Features:Logging:Disabled", bool.TrueString }
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(keys)
                .Build();
            var assembly = new Mock<Assembly>();

            // Act
            var services = AppFeatureFactory.GetAppFeatures(configuration, [assembly.Object]);

            // Assert
            Assert.That(services, Is.Empty);
        }

        [Test(Description = "Если в сборке нет функциональности объявленной в конфигурации, то должен быть пустой список")]
        public void AppFeature_AssemblyNotContainFeature_AppFeaturesIsEmpty()
        {
            // Arrange
            var keys = new Dictionary<string, string?>
            {
               { "Features", string.Empty },
               { "Features:Logging", string.Empty }
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(keys)
                .Build();
            var assembly = new Mock<Assembly>();
            assembly.Setup(a => a.GetTypes()).Returns([typeof(HealthCheckFeature)]);

            // Act
            var services = AppFeatureFactory.GetAppFeatures(configuration, [assembly.Object]);

            // Assert
            Assert.That(services, Is.Empty);
        }

        [Test(Description = "Если функциональность есть в сборке и есть в конфигурации, то коллекции сервисов не пустая")]
        public void AppFeature_AssemblyContainFeature_ServiceCollectionContainAppFeature()
        {
            // Arrange
            var keys = new Dictionary<string, string?>
            {
               { "Features", string.Empty },
               { "Features:HealthCheck", string.Empty }
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(keys)
                .Build();
            var assembly = new Mock<Assembly>();
            assembly.Setup(a => a.GetTypes()).Returns([typeof(HealthCheckFeature)]);

            // Act
            var services = AppFeatureFactory.GetAppFeatures(configuration, [assembly.Object]);

            // Assert
            var feature = services.FirstOrDefault();
            Assert.Multiple(() =>
            {
                Assert.That(services, Is.Not.Empty);
                Assert.That(feature, Is.Not.Null);
            });
            Assert.That(feature!.GetType(), Is.EqualTo(typeof(HealthCheckFeature)));
        }
    }
}
