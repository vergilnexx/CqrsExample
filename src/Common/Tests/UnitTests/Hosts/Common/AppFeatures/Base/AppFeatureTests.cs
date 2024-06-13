using Meta.Common.Hosts.Features.AppFeatures.Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using static Google.Protobuf.Compiler.CodeGeneratorResponse.Types;

namespace Meta.Common.Test.UnitTests.Hosts.Common.AppFeatures.Base
{
    /// <summary>
    /// Тесты базового класса функциональностей.
    /// </summary>
    internal class AppFeatureTests
    {
        [Test(Description = "При инициализации не должно быть выбрасываний исключений.")]
        public void Init_Valid_NoException()
        {
            // Arrange
            var appFeature = new Mock<AppFeature>() { CallBase = true };
            var request = new AppFeatureInitRequest()
            {
                Name = string.Empty,
                Order = 0,
                Configuration = new Mock<IConfiguration>().Object,
                OptionSection = new Mock<IConfigurationSection>().Object,
                AdditionalAssemblies = [],
            };

            // Act
            // Assert
            Assert.DoesNotThrow(() =>
            {
                appFeature.Object.Init(request);
            });
        }

        [Test(Description = "При базовом добавлении фичи, коллекция остается пустой.")]
        public void AddFeature_Valid_ServiceCollectionIsEmpty()
        {
            // Arrange
            var services = new ServiceCollection();
            var appFeature = new Mock<AppFeature>() { CallBase = true };

            // Act
            appFeature.Object.AddFeature(services);

            // Assert
            Assert.That(services, Is.Empty);
        }

        [Test(Description = "При базовом добавлении фичи, коллекция остается пустой.")]
        public void AddFeatureWithBuilder_Valid_ServiceCollectionIsEmpty()
        {
            // Arrange
            var services = new ServiceCollection();
            var hostBuilder = new Mock<IHostBuilder>();
            var loggingBuilder = new Mock<ILoggingBuilder>();
            var appFeature = new Mock<AppFeature>() { CallBase = true };

            // Act
            appFeature.Object.AddFeature(services, hostBuilder.Object, loggingBuilder.Object);

            // Assert
            Assert.That(services, Is.Empty);
        }

        [Test(Description = "При базовом добавлении фичи, коллекция остается пустой.")]
        public void AddFeatureWithApplicationBuilder_Valid_ServiceCollectionIsEmpty()
        {
            // Arrange
            var services = new ServiceCollection();
            var hostBuilder = new Mock<IHostApplicationBuilder>();
            var loggingBuilder = new Mock<ILoggingBuilder>();
            var appFeature = new Mock<AppFeature>() { CallBase = true };

            // Act
            appFeature.Object.AddFeature(services, hostBuilder.Object, loggingBuilder.Object);

            // Assert
            Assert.That(services, Is.Empty);
        }

        [Test(Description = "При базовом использовании точек доступа, не должно быть никаких вызовов.")]
        public void UseEndpoints_Valid_NoOtherCalls()
        {
            // Arrange
            var builder = new Mock<IEndpointRouteBuilder>();
            var appFeature = new Mock<AppFeature>() { CallBase = true };

            // Act
            appFeature.Object.UseEndpoints(builder.Object);

            // Assert
            builder.VerifyNoOtherCalls();
        }

        [Test(Description = "При базовом использовании функциональности, не должно быть никаких вызовов.")]
        public void UseFeature_Valid_NoOtherCalls()
        {
            // Arrange
            var builder = new Mock<IApplicationBuilder>();
            var environment = new Mock<IWebHostEnvironment>();
            var appFeature = new Mock<AppFeature>() { CallBase = true };

            // Act
            appFeature.Object.UseFeature(builder.Object, environment.Object);

            // Assert
            builder.VerifyNoOtherCalls();
            environment.VerifyNoOtherCalls();
        }
    }
}
