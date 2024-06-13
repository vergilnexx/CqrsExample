using Meta.Common.Hosts.Api.Features.AppFeatures.HealthCheck.Instances.Api;
using Meta.Common.Hosts.Features.AppFeatures.Base;
using Meta.Common.Hosts.Features.AppFeatures.HealthCheck;
using Meta.Common.Hosts.Features.AppFeatures.HealthCheck.Instances.RabbitMq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;
using System.Reflection;
using System.Xml.Linq;

namespace Meta.Common.Test.UnitTests.Hosts.Common.AppFeatures.HealthCheck
{
    /// <summary>
    /// Тесты функциональности HealthCheck'ов.
    /// </summary>
    internal class HealthCheckFeatureTests
    {
        internal class TestHealthCheckFeature : HealthCheckFeature
        {
            internal Func<HealthCheckRegistration, bool> TestPredicateByTag(string tag)
            {
                return PredicateByTag(tag);
            }

            internal void TestWriteResponse(HttpContext context, HealthReport report)
            {
                WriteResponse(context, report);
            }
        }

        [Test(Description = "В функциональности не инициализирована секция настроек - в итоге функциональность не зарегистрирована в коллекции сервисов")]
        public void AddFeature_OptionSectionNull_ServiceCollectionIsEmpty()
        {
            // Arrange
            var feature = new HealthCheckFeature();
            var services = new ServiceCollection();

            // Act
            feature.AddFeature(services);

            // Assert
            Assert.That(
                services.Any(s => s.ServiceType.FullName == "Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckService"),
                Is.False);
        }

        [Test(Description = "В функциональности нет секций настроек healthCheck'ов - в итоге функциональность не зарегистрирована в коллекции сервисов")]
        public void AddFeature_HealthCheckSectionsIsEmpty_ServiceCollectionIsEmpty()
        {
            // Arrange
            var keys = new Dictionary<string, string?>();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var initRequest = new AppFeatureInitRequest()
            {
                Order = 0,
                Name = "test",
                Configuration = configuration,
                OptionSection = configuration.GetSection("test"),
                AdditionalAssemblies = []
            };
            var feature = new HealthCheckFeature();
            feature.Init(initRequest);
            var services = new ServiceCollection();

            // Act
            feature.AddFeature(services);

            // Assert
            Assert.That(
                services.Any(s => s.ServiceType.FullName == "Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckService"),
                Is.False);
        }

        [Test(Description = "Задана секция в HealthCheck, но нет настроек, в итоге функциональность не зарегистрирована в коллекции сервисов")]
        public void AddFeature_HealthCheckSectionOptionsIsEmpty_ServiceCollectionIsEmpty()
        {
            // Arrange
            var keys = new Dictionary<string, string?>()
            {
                { "Features", string.Empty },
                { "Features:HealthCheck", string.Empty },
                { "Features:HealthCheck:Sections", string.Empty },
                { "Features:HealthCheck:Sections:0", string.Empty }
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var assembly = new Mock<Assembly>();
            var initRequest = new AppFeatureInitRequest()
            {
                Order = 0,
                Name = "Api",
                Configuration = configuration,
                OptionSection = configuration.GetSection("Features:HealthCheck"),
                AdditionalAssemblies = [assembly.Object]
            };
            var feature = new HealthCheckFeature();
            feature.Init(initRequest);
            var services = new ServiceCollection();

            // Act
            feature.AddFeature(services);

            // Assert
            Assert.That(
                services.Any(s => s.ServiceType.FullName == "Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckService"),
                Is.False);
        }

        [Test(Description = "Задана секция в HealthCheck, но она выключена, в итоге функциональность не зарегистрирована в коллекции сервисов")]
        public void AddFeature_HealthCheckSectionDisabled_ServiceCollectionIsEmpty()
        {
            // Arrange
            var keys = new Dictionary<string, string?>()
            {
                { "Features", string.Empty },
                { "Features:HealthCheck", string.Empty },
                { "Features:HealthCheck:Sections", string.Empty },
                { "Features:HealthCheck:Sections:0", string.Empty },
                { "Features:HealthCheck:Sections:0:Name", "Api" },
                { "Features:HealthCheck:Sections:0:Disabled", bool.TrueString },
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var assembly = new Mock<Assembly>();
            var initRequest = new AppFeatureInitRequest()
            {
                Order = 0,
                Name = "Api",
                Configuration = configuration,
                OptionSection = configuration.GetSection("Features:HealthCheck"),
                AdditionalAssemblies = [assembly.Object]
            };
            var feature = new HealthCheckFeature();
            feature.Init(initRequest);
            var services = new ServiceCollection();

            // Act
            feature.AddFeature(services);

            // Assert
            Assert.That(
                services.Any(s => s.ServiceType.FullName == "Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckService"), 
                Is.False);
        }

        [Test(Description = "Задана секция в HealthCheck, но она не найдена в сборке, в итоге функциональность не зарегистрирована в коллекции сервисов")]
        public void AddFeature_HealthCheckSectionNotFoundInAssembly_ServiceCollectionIsEmpty()
        {
            // Arrange
            var keys = new Dictionary<string, string?>()
            {
                { "Features", string.Empty },
                { "Features:HealthCheck", string.Empty },
                { "Features:HealthCheck:Sections", string.Empty },
                { "Features:HealthCheck:Sections:0", string.Empty },
                { "Features:HealthCheck:Sections:0:Name", "Api" },
                { "Features:HealthCheck:Sections:0:Options", string.Empty }
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var assembly = new Mock<Assembly>();
            assembly.Setup(a => a.GetTypes()).Returns([typeof(RabbitMqHealthCheckConfigurator)]);
            var initRequest = new AppFeatureInitRequest()
            {
                Order = 0,
                Name = "Api",
                Configuration = configuration,
                OptionSection = configuration.GetSection("Features:HealthCheck"),
                AdditionalAssemblies = [assembly.Object]
            };
            var feature = new HealthCheckFeature();
            feature.Init(initRequest);
            var services = new ServiceCollection();

            // Act
            feature.AddFeature(services);

            // Assert
            Assert.That(
                services.Any(s => s.ServiceType.FullName == "Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckService"),
                Is.False);
        }

        [Test(Description = "Задана секция в HealthCheck - функциональность зарегистрирована в коллекции сервисов")]
        public void AddFeature_HealthCheckSectionFoundInAssembly_HealthCheckSectionConfigured()
        {
            // Arrange
            var keys = new Dictionary<string, string?>()
            {
                { "Features", string.Empty },
                { "Features:HealthCheck", string.Empty },
                { "Features:HealthCheck:Sections", string.Empty },
                { "Features:HealthCheck:Sections:0", string.Empty },
                { "Features:HealthCheck:Sections:0:Name", "Api" },
                { "Features:HealthCheck:Sections:0:Options", string.Empty }
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var assembly = new Mock<Assembly>();
            assembly.Setup(a => a.GetTypes()).Returns([typeof(ApiHealthCheckConfigurator)]);
            var initRequest = new AppFeatureInitRequest()
            {
                Order = 0,
                Name = "Api",
                Configuration = configuration,
                OptionSection = configuration.GetSection("Features:HealthCheck"),
                AdditionalAssemblies = [assembly.Object]
            };
            var feature = new HealthCheckFeature();
            feature.Init(initRequest);
            var services = new ServiceCollection();

            // Act
            feature.AddFeature(services);

            // Assert
            Assert.That(
                services.Any(s => s.ServiceType.FullName == "Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckService"),
                Is.True);
        }

        [Test(Description = "В функциональности не инициализирована секция настроек - в итоге функциональность не зарегистрирована в коллекции сервисов")]
        public void AddFeatureWithBuilder_OptionSectionNull_ServiceCollectionIsEmpty()
        {
            // Arrange
            var feature = new HealthCheckFeature();
            var services = new ServiceCollection();
            var hostBuilder = new Mock<IHostBuilder>();
            var loggingBuilder = new Mock<ILoggingBuilder>();

            // Act
            feature.AddFeature(services, hostBuilder.Object, loggingBuilder.Object);

            // Assert
            Assert.That(
                services.Any(s => s.ServiceType.FullName == "Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckService"),
                Is.False);
        }

        [Test(Description = "В функциональности не инициализирована секция настроек - в итоге функциональность не зарегистрирована в коллекции сервисов")]
        public void AddFeatureWithApplicationBuilder_OptionSectionNull_ServiceCollectionIsEmpty()
        {
            // Arrange
            var feature = new HealthCheckFeature();
            var services = new ServiceCollection();
            var hostBuilder = new Mock<IHostApplicationBuilder>();
            var loggingBuilder = new Mock<ILoggingBuilder>();

            // Act
            feature.AddFeature(services, hostBuilder.Object, loggingBuilder.Object);

            // Assert
            Assert.That(
                services.Any(s => s.ServiceType.FullName == "Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckService"),
                Is.False);
        }

        [Test(Description = "Если функциональность выключена, то сервисы не должны быть зарегистрированы")]
        public void UseEndpoints_Disabled_ServiceCollectionIsEmpty()
        {
            // Arrange
            var keys = new Dictionary<string, string?>()
            {
                { "Features", string.Empty },
                { "Features:HealthCheck", string.Empty },
                { "Features:HealthCheck:Sections", string.Empty },
                { "Features:HealthCheck:Sections:0", string.Empty },
                { "Features:HealthCheck:Sections:0:Disabled", bool.TrueString },
                { "Features:HealthCheck:Sections:0:Name", "Api" },
                { "Features:HealthCheck:Sections:0:Options", string.Empty }
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var assembly = new Mock<Assembly>();
            assembly.Setup(a => a.GetTypes()).Returns([typeof(ApiHealthCheckConfigurator)]);
            var initRequest = new AppFeatureInitRequest()
            {
                Order = 0,
                Name = "Api",
                Configuration = configuration,
                OptionSection = configuration.GetSection("Features:HealthCheck"),
                AdditionalAssemblies = [assembly.Object]
            };
            var feature = new HealthCheckFeature();
            feature.Init(initRequest);
            var routeBuilder = new Mock<IEndpointRouteBuilder>();
            var serviceProvider = new Mock<IServiceProvider>();
            var healthCheckService = new Mock<HealthCheckService>();
            var appBuilder = new ApplicationBuilder(serviceProvider.Object);
            serviceProvider.Setup(sp => sp.GetService(It.Is<Type>(t => t == typeof(HealthCheckService))))
                           .Returns(healthCheckService.Object);
            routeBuilder.Setup(b => b.ServiceProvider).Returns(serviceProvider.Object);
            routeBuilder.Setup(b => b.CreateApplicationBuilder()).Returns(appBuilder);
            var sources = new Mock<ICollection<EndpointDataSource>>();
            routeBuilder.Setup(b => b.DataSources).Returns(sources.Object);
            sources
                .Setup(b => b.GetEnumerator())
                .Returns(GetSources());

            // Act
            feature.UseEndpoints(routeBuilder.Object);

            // Assert
            sources.Verify(s => s.Add(It.IsAny<EndpointDataSource>()), Times.Exactly(0));
        }

        [Test(Description = "Если в сборке нет конфигуратора, то сервисы не должны быть зарегистрированы")]
        public void UseEndpoints_AssemblyHasNotConfigurator_ServiceCollectionIsEmpty()
        {
            // Arrange
            var keys = new Dictionary<string, string?>()
            {
                { "Features", string.Empty },
                { "Features:HealthCheck", string.Empty },
                { "Features:HealthCheck:Sections", string.Empty },
                { "Features:HealthCheck:Sections:0", string.Empty },
                { "Features:HealthCheck:Sections:0:Name", "Api" },
                { "Features:HealthCheck:Sections:0:Options", string.Empty }
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var assembly = new Mock<Assembly>();
            var initRequest = new AppFeatureInitRequest()
            {
                Order = 0,
                Name = "Api",
                Configuration = configuration,
                OptionSection = configuration.GetSection("Features:HealthCheck"),
                AdditionalAssemblies = [assembly.Object]
            };
            var feature = new HealthCheckFeature();
            feature.Init(initRequest);
            var routeBuilder = new Mock<IEndpointRouteBuilder>();
            var serviceProvider = new Mock<IServiceProvider>();
            var healthCheckService = new Mock<HealthCheckService>();
            var appBuilder = new ApplicationBuilder(serviceProvider.Object);
            serviceProvider.Setup(sp => sp.GetService(It.Is<Type>(t => t == typeof(HealthCheckService))))
                           .Returns(healthCheckService.Object);
            routeBuilder.Setup(b => b.ServiceProvider).Returns(serviceProvider.Object);
            routeBuilder.Setup(b => b.CreateApplicationBuilder()).Returns(appBuilder);
            var sources = new Mock<ICollection<EndpointDataSource>>();
            routeBuilder.Setup(b => b.DataSources).Returns(sources.Object);
            sources
                .Setup(b => b.GetEnumerator())
                .Returns(GetSources());

            // Act
            feature.UseEndpoints(routeBuilder.Object);

            // Assert
            sources.Verify(s => s.Add(It.IsAny<EndpointDataSource>()), Times.Exactly(0));
        }

        [Test(Description = "Созданы точки доступа для проверки готовности")]
        public void UseEndpoints_Valid_ReadinessMapHealthCheck()
        {
            // Arrange
            var keys = new Dictionary<string, string?>()
            {
                { "Features", string.Empty },
                { "Features:HealthCheck", string.Empty },
                { "Features:HealthCheck:Sections", string.Empty },
                { "Features:HealthCheck:Sections:0", string.Empty },
                { "Features:HealthCheck:Sections:0:Name", "Api" },
                { "Features:HealthCheck:Sections:0:Options", string.Empty }
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var assembly = new Mock<Assembly>();
            assembly.Setup(a => a.GetTypes()).Returns([typeof(ApiHealthCheckConfigurator)]);
            var initRequest = new AppFeatureInitRequest()
            {
                Order = 0,
                Name = "Api",
                Configuration = configuration,
                OptionSection = configuration.GetSection("Features:HealthCheck"),
                AdditionalAssemblies = [assembly.Object]
            };
            var feature = new HealthCheckFeature();
            feature.Init(initRequest);
            var routeBuilder = new Mock<IEndpointRouteBuilder>();
            var serviceProvider = new Mock<IServiceProvider>();
            var healthCheckService = new Mock<HealthCheckService>();
            var appBuilder = new ApplicationBuilder(serviceProvider.Object);
            serviceProvider.Setup(sp => sp.GetService(It.Is<Type>(t => t == typeof(HealthCheckService))))
                           .Returns(healthCheckService.Object);
            routeBuilder.Setup(b => b.ServiceProvider).Returns(serviceProvider.Object);
            routeBuilder.Setup(b => b.CreateApplicationBuilder()).Returns(appBuilder);
            var sources = new Mock<ICollection<EndpointDataSource>>();
            routeBuilder.Setup(b => b.DataSources).Returns(sources.Object);
            sources
                .Setup(b => b.GetEnumerator())
                .Returns(GetSources());

            // Act
            feature.UseEndpoints(routeBuilder.Object);

            // Assert
            sources.Verify(s => s.Add(It.IsAny<EndpointDataSource>()), Times.Exactly(2));
        }

        [Test(Description = "При формировании ответа о том что сервис не здоров, ответ должен быть 503")]
        public void WriteResponse_Unhealthy_ResponseStatusIsServiceUnavailable()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var report = new HealthReport(new Dictionary<string, HealthReportEntry>(), HealthStatus.Unhealthy, TimeSpan.Zero);
            var feature = new TestHealthCheckFeature();

            // Act
            feature.TestWriteResponse(context, report);

            // Assert
            Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status503ServiceUnavailable));
        }

        [Test(Description = "При формировании ответа о том что сервис здоров, ответ должен быть 200")]
        public void WriteResponse_Healthy_ResponseIsOk()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var report = new HealthReport(new Dictionary<string, HealthReportEntry>(), HealthStatus.Healthy, TimeSpan.Zero);
            var feature = new TestHealthCheckFeature();

            // Act
            feature.TestWriteResponse(context, report);

            // Assert
            Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        }

        [Test(Description = "При формировании ответа о том что сервис нагружен, ответ должен быть 200")]
        public void WriteResponse_Degraded_ResponseIsOk()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var report = new HealthReport(new Dictionary<string, HealthReportEntry>(), HealthStatus.Degraded, TimeSpan.Zero);
            var feature = new TestHealthCheckFeature();

            // Act
            feature.TestWriteResponse(context, report);

            // Assert
            Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        }

        [Test(Description = "При формировании ответа о том что сервис нагружен, ответ не должен быть неопределенным")]
        public void WriteResponse_ReportFilled_ResponseNotNull()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var dictionary = new Dictionary<string, HealthReportEntry>()
            {
                {
                    "atLeastOne",
                    new HealthReportEntry(HealthStatus.Degraded, "description", TimeSpan.Zero, exception: null, new Dictionary<string, object>(), ["atLeastOne"])
                }
            };
            var report = new HealthReport(dictionary, HealthStatus.Unhealthy, TimeSpan.Zero);
            var feature = new TestHealthCheckFeature();

            // Act
            feature.TestWriteResponse(context, report);

            // Assert
            Assert.That(context.Response, Is.Not.Null);
        }

        [Test(Description = "При одинаковых тэгах, результат проверки возвращает истину")]
        public void PredicateByTags_TagTheSame_True()
        {
            // Arrange
            var feature = new TestHealthCheckFeature();
            var tag = "atLeastOne";

            // Act
            var predicate = feature.TestPredicateByTag(tag);
            var result = predicate.Invoke(new HealthCheckRegistration("name", new Mock<IHealthCheck>().Object, HealthStatus.Unhealthy, [tag]));

            // Assert
            Assert.That(result, Is.True);
        }

        [Test(Description = "При разных тэгах, результат проверки возвращает ложь")]
        public void PredicateByTags_TagTheSame_False()
        {
            // Arrange
            var feature = new TestHealthCheckFeature();
            var tag = "atLeastOne";

            // Act
            var predicate = feature.TestPredicateByTag(tag);
            var result = predicate.Invoke(new HealthCheckRegistration("name", new Mock<IHealthCheck>().Object, HealthStatus.Unhealthy, ["other"]));

            // Assert
            Assert.That(result, Is.False);
        }

        private static IEnumerator<EndpointDataSource> GetSources()
        {
            yield break;
        }
    }
}
