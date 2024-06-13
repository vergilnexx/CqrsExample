using Meta.Common.Hosts.Api.Features.AppFeatures.OpenTelemetry;
using Meta.Common.Hosts.Features.AppFeatures.Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;

namespace Meta.Common.Tests.UnitTests.Hosts.Api.Features.OpenTelemetry
{
    /// <summary>
    /// Тесты функциональности OpenTelemetry
    /// </summary>
    internal class OpenTelemetryFeatureTests
    {
        [Test(Description = "При добавлении функциональности и не заданной секции конфигурации, коллекция сервисов должна быть пустой")]
        public void AddFeature_OptionSetcionIsNull_ServceCollectionIsEmpty()
        {
            // Arrange
            var feature = new OpenTelemetryFeature();
            var services = new ServiceCollection();
            var hostBuilder = new Mock<IHostBuilder>();
            var loggingBuilder = new Mock<ILoggingBuilder>();

            // Act
            feature.AddFeature(services, hostBuilder.Object, loggingBuilder.Object);

            // Assert
            Assert.That(services, Is.Empty);
        }

        [Test(Description = "При использовании функциональности и не заданной секции конфигурации, методы не вызываются")]
        public void UseEndpoints_OptionSetcionIsNull_NoOtherCalls()
        {
            // Arrange
            var feature = new OpenTelemetryFeature();
            var builder = new Mock<IEndpointRouteBuilder>();

            // Act
            feature.UseEndpoints(builder.Object);

            // Assert
            builder.VerifyNoOtherCalls();
        }

        [Test(Description = "При добавлении функциональности, коллекция сервисов не должна быть пустой")]
        public void AddFeature_ValidOptions_ServceCollectionIsNotEmpty()
        {
            // Arrange
            var feature = new OpenTelemetryFeature();
            var keys = new Dictionary<string, string?>()
            {
                { "Features", string.Empty },
                { "Features:OpenTelemetry", string.Empty },
                { "Features:OpenTelemetry:Order", 0.ToString() },
                { "Features:OpenTelemetry:ApplicationName", "example" },
                { "Features:OpenTelemetry:PrometheusEnabled", bool.TrueString },
                { "Features:OpenTelemetry:ExporterOtlpUrl", "http://localhost:4317" },
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var initRequest = new AppFeatureInitRequest()
            {
                Order = 0,
                Name = "Features",
                Configuration = configuration,
                OptionSection = configuration.GetSection("Features:OpenTelemetry"),
                AdditionalAssemblies = []
            };
            feature.Init(initRequest);
            var services = new ServiceCollection();
            var hostBuilder = new Mock<IHostBuilder>();
            var loggingBuilder = new Mock<ILoggingBuilder>();
            loggingBuilder.SetupGet(b => b.Services).Returns(new ServiceCollection());

            // Act
            feature.AddFeature(services, hostBuilder.Object, loggingBuilder.Object);

            // Assert
            Assert.That(services, Is.Not.Empty);
        }

        [Test(Description = "При использовании функциональности и заданной секции конфигурации, нет исключений")]
        public void UseEndpoints_OptionSectionIsNotNull_NoException()
        {
            // Arrange
            var feature = new OpenTelemetryFeature();
            var keys = new Dictionary<string, string?>()
            {
                { "Features", string.Empty },
                { "Features:OpenTelemetry", string.Empty },
                { "Features:OpenTelemetry:Order", 0.ToString() },
                { "Features:OpenTelemetry:ApplicationName", "example" },
                { "Features:OpenTelemetry:PrometheusEnabled", bool.TrueString },
                { "Features:OpenTelemetry:ExporterOtlpUrl", "http://localhost:4317" },
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var initRequest = new AppFeatureInitRequest()
            {
                Order = 0,
                Name = "Features",
                Configuration = configuration,
                OptionSection = configuration.GetSection("Features:OpenTelemetry"),
                AdditionalAssemblies = []
            };
            feature.Init(initRequest);
            var builder = WebApplication.CreateBuilder();
            var services = builder.Services;
            feature.AddFeature(services, builder.Host, builder.Logging);
            var app = builder.Build();

            // Act
            // Assert
            Assert.DoesNotThrow(() =>
                feature.UseEndpoints(app));
        }
    }
}
