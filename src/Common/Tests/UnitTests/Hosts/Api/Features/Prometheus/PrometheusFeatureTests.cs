using Meta.Common.Hosts.Api.Features.AppFeatures.Prometheus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Moq;

namespace Meta.Common.Tests.UnitTests.Hosts.Api.Features.Prometheus
{
    /// <summary>
    /// Тесты функциональности Prometheus.
    /// </summary>
    internal class PrometheusFeatureTests
    {
        [Test(Description = "При валидных настройках, не должно быть исключений")]
        public void UseFeature_ValidOptions_NoExceptions()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            var app = builder.Build();
            var feature = new PrometheusFeature();
            var environment = new Mock<IWebHostEnvironment>();

            // Act
            // Assert
            Assert.DoesNotThrow(() =>
                feature.UseFeature(app, environment.Object));
        }
    }
}
