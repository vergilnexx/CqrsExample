using Google.Protobuf.WellKnownTypes;
using Meta.Common.Hosts.Api.Features.AppFeatures.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Meta.Common.Tests.UnitTests.Hosts.Api.Features.Swagger
{
    /// <summary>
    /// Тесты функциональности Swagger
    /// </summary>
    internal class SwaggerFeatureTests
    {
        [Test(Description = "При регистрации функциональности, коллекция сервисов не должна быть пустой")]
        public void AddFeature_Valid_ServiceCollectionIsNotEmpty()
        {
            // Arrange
            var feature = new SwaggerFeature();
            var services = new ServiceCollection();
            var hostBuilder = new Mock<IHostBuilder>();
            var loggingBuilder = new Mock<ILoggingBuilder>();

            // Act
            feature.AddFeature(services, hostBuilder.Object, loggingBuilder.Object);

            // Assert
            Assert.That(services, Is.Not.Empty);
        }

        [Test(Description = "При использовании функциональности, не должно быть исключений")]
        public void UseFeature_Valid_NoException()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            var app = builder.Build();
            var feature = new SwaggerFeature();
            var environment = new Mock<IWebHostEnvironment>();
            environment.SetupGet(e => e.EnvironmentName).Returns("Development");

            // Act
            // Assert
            Assert.DoesNotThrow(() =>
                feature.UseFeature(app, environment.Object));
        }

        [Test(Description = "При конфигурировании функциональности, не должно быть исключений")]
        public void Configure_Valid_NoException()
        {
            // Arrange
            var feature = new SwaggerFeature();
            var environment = new Mock<IWebHostEnvironment>();
            environment.SetupGet(e => e.EnvironmentName).Returns("Development");
            var options = new Mock<SwaggerGenOptions>();

            // Act
            // Assert
            Assert.DoesNotThrow(() =>
                feature.Configure(options.Object));
        }
    }
}
