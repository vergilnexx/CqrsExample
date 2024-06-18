using Meta.Common.Hosts.Api.Features.AppFeatures.HealthCheck.Instances.Api;
using Meta.Common.Hosts.Features.AppFeatures.HealthCheck.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;

namespace Meta.Common.Tests.UnitTests.Hosts.Api.Features.HealthCheck.Api
{
    /// <summary>
    /// Тесты функциональности healthCheck'ов API.
    /// </summary>
    internal class ApiHealthCheckTests
    {
        [Test(Description = "Если при проверке таймаут не задан, то выбрасывается исключение")]
        public void CheckHealthAsync_TimeoutOptionIsNotParsed_HealthCheckConfigurationException()
        {
            // Arrange
            var keys = new Dictionary<string, string?>();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var options = new ApiHealthCheckOptions()
            {
                Name = "default",
                UrlConfigSection = "section",
                Timeout = "timeout"
            };
            var logger = new Mock<ILogger<ApiHealthCheck>>();
            var check = new ApiHealthCheck(configuration, httpClientFactory.Object, options, logger.Object);
            var context = new HealthCheckContext();

            // Act
            var exception = Assert.ThrowsAsync<HealthCheckConfigurationException>(
                async () => await check.CheckHealthAsync(context, CancellationToken.None));

            // Assert
            Assert.That(exception?.Message.Contains("Timeout"), Is.True);
        }

        [Test(Description = "Если при проверке секция с настройками API не задана, то выбрасывается исключение")]
        public void CheckHealthAsync_UrlConfigSectionOptionIsEmpty_HealthCheckConfigurationException()
        {
            // Arrange
            var keys = new Dictionary<string, string?>();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var options = new ApiHealthCheckOptions()
            {
                Name = "default",
                UrlConfigSection = string.Empty,
            };
            var logger = new Mock<ILogger<ApiHealthCheck>>();
            var check = new ApiHealthCheck(configuration, httpClientFactory.Object, options, logger.Object);
            var context = new HealthCheckContext();

            // Act
            var exception = Assert.ThrowsAsync<HealthCheckConfigurationException>(
                async () => await check.CheckHealthAsync(context, CancellationToken.None));

            // Assert
            Assert.That(exception?.Message, Is.EqualTo("Название секции с URL для проверки API не может быть пустой."));
        }

        [Test(Description = "Если при проверке настройки API пусты, то выбрасывается исключение")]
        public void CheckHealthAsync_SectionOptionIsEmpty_HealthCheckConfigurationException()
        {
            // Arrange
            var keys = new Dictionary<string, string?>()
            {
                { "section", string.Empty },
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var options = new ApiHealthCheckOptions()
            {
                Name = "default",
                UrlConfigSection = "section",
            };
            var logger = new Mock<ILogger<ApiHealthCheck>>();
            var check = new ApiHealthCheck(configuration, httpClientFactory.Object, options, logger.Object);
            var context = new HealthCheckContext();

            // Act
            var exception = Assert.ThrowsAsync<HealthCheckConfigurationException>(
                async () => await check.CheckHealthAsync(context, CancellationToken.None));

            // Assert
            Assert.That(exception?.Message.Contains("section"), Is.True);
        }

        [Test(Description = "Если при проверке URL API не является URL, то выбрасывается исключение")]
        public void CheckHealthAsync_SectionOptionIsNotUrl_HealthCheckConfigurationException()
        {
            // Arrange
            var keys = new Dictionary<string, string?>()
            {
                { "section", "localhost" },
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var options = new ApiHealthCheckOptions()
            {
                Name = "default",
                UrlConfigSection = "section",
            };
            var logger = new Mock<ILogger<ApiHealthCheck>>();
            var check = new ApiHealthCheck(configuration, httpClientFactory.Object, options, logger.Object);
            var context = new HealthCheckContext();

            // Act
            var exception = Assert.ThrowsAsync<HealthCheckConfigurationException>(
                async () => await check.CheckHealthAsync(context, CancellationToken.None));

            // Assert
            Assert.That(exception?.Message.Contains("невалидный формат для проверки API"), Is.True);
        }

        [Test(Description = "Если запрос вернул 400, то сервис Unhealthy")]
        public async Task CheckHealthAsync_ResponseIsBadRequest_ServiceStatusIsUnhealthy()
        {
            // Arrange
            var keys = new Dictionary<string, string?>()
            {
                { "section", "http://localhost:5101" }
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttp = new Mock<HttpMessageHandler>();
            mockHttp.Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.Is<HttpRequestMessage>(m => m.Method == HttpMethod.Get),
                        ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.BadRequest });
            var httpClient = new HttpClient(mockHttp.Object);
            httpClientFactory.Setup(f => f.CreateClient(Options.DefaultName)).Returns(httpClient);
            var options = new ApiHealthCheckOptions()
            {
                Name = "default",
                UrlConfigSection = "section",
            };
            var logger = new Mock<ILogger<ApiHealthCheck>>();
            var check = new ApiHealthCheck(configuration, httpClientFactory.Object, options, logger.Object);
            var context = new HealthCheckContext();

            // Act
            var result = await check.CheckHealthAsync(context, CancellationToken.None);

            // Assert
            Assert.That(result.Status, Is.EqualTo(HealthStatus.Unhealthy));
        }


        [Test(Description = "Если запрос вернул 500, то сервис Unhealthy")]
        public async Task CheckHealthAsync_ResponseIsInternalServerError_ServiceStatusIsUnhealthy()
        {
            // Arrange
            var keys = new Dictionary<string, string?>()
            {
                { "section", "http://localhost:5101" }
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttp = new Mock<HttpMessageHandler>();
            mockHttp.Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.Is<HttpRequestMessage>(m => m.Method == HttpMethod.Get),
                        ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.InternalServerError });
            var httpClient = new HttpClient(mockHttp.Object);
            httpClientFactory.Setup(f => f.CreateClient(Options.DefaultName)).Returns(httpClient);
            var options = new ApiHealthCheckOptions()
            {
                Name = "default",
                UrlConfigSection = "section",
            };
            var logger = new Mock<ILogger<ApiHealthCheck>>();
            var check = new ApiHealthCheck(configuration, httpClientFactory.Object, options, logger.Object);
            var context = new HealthCheckContext();

            // Act
            var result = await check.CheckHealthAsync(context, CancellationToken.None);

            // Assert
            Assert.That(result.Status, Is.EqualTo(HealthStatus.Unhealthy));
        }

        [Test(Description = "Если запрос вернул 200, то сервис Healthy")]
        public async Task CheckHealthAsync_ResponseIsOk_ServiceStatusIsHealthy()
        {
            // Arrange
            var keys = new Dictionary<string, string?>()
            {
                { "section", "http://localhost:5101" }
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttp = new Mock<HttpMessageHandler>();
            mockHttp.Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.Is<HttpRequestMessage>(m => m.Method == HttpMethod.Get),
                        ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.OK });
            var httpClient = new HttpClient(mockHttp.Object);
            httpClientFactory.Setup(f => f.CreateClient(Options.DefaultName)).Returns(httpClient);
            var options = new ApiHealthCheckOptions()
            {
                Name = "default",
                UrlConfigSection = "section",
            };
            var logger = new Mock<ILogger<ApiHealthCheck>>();
            var check = new ApiHealthCheck(configuration, httpClientFactory.Object, options, logger.Object);
            var context = new HealthCheckContext();

            // Act
            var result = await check.CheckHealthAsync(context, CancellationToken.None);

            // Assert
            Assert.That(result.Status, Is.EqualTo(HealthStatus.Healthy));
        }
    }
}
