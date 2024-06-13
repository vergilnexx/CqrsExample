using Meta.Common.Hosts.Features.AppFeatures.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;

namespace Meta.Common.Tests.UnitTests.Hosts.Api.Features.Routing
{
    /// <summary>
    /// Тесты роутинга.
    /// </summary>
    internal class RoutingFeatureTests
    {
        [Test(Description = "При добавлении функциональности, коллекция сервисов не должна быть пустой.")]
        public void AddFeature_ValidOptions_ServiceCollectionIsNotEmpty()
        {
            // Arrange
            var services = new ServiceCollection();
            var feature = new RoutingFeature();
            var hostBuilder = new Mock<IHostBuilder>();
            var loggingBuilder = new Mock<ILoggingBuilder>();

            // Act
            feature.AddFeature(services, hostBuilder.Object, loggingBuilder.Object);

            //Assert
            Assert.That(services, Is.Not.Empty);
        }

        [Test(Description = "При использовании функциональности, не должно быть исключений.")]
        public void UseFeature_ValidOptions_NoException()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            var app = builder.Build();
            var feature = new RoutingFeature();
            var environment = new Mock<IWebHostEnvironment>();

            // Act
            // Assert
            Assert.DoesNotThrow(() => feature.UseFeature(app, environment.Object));
        }
    }
}
