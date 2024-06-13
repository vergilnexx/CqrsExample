using Meta.Common.Contracts.Options;
using Meta.Common.Hosts.Features.AppFeatures.Authorization.Instances.TrustedNetwork;
using Meta.Common.Hosts.Features.AppFeatures.Authorization.Instances.TrustedNetwork.Models;
using Meta.Common.Hosts.Features.AppFeatures.Authorization.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Moq;
using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Meta.Common.Tests.UnitTests.Hosts.Api.Features.Authorization
{
    internal class TrustedNetworkAuthorizationHandlerTests
    {
        [Test(Description = "При обработке проверки доверенных сетей, выбрасывается исключение если не авторизован.")]
        public async Task HandleAuthenticateAsync_NoAuthorize_AuthorizationException()
        {
            // Arrange
            var schemeName = "SchemeName";
            var options = new Mock<IOptionsMonitor<TrustedNetworkOptions>>();
            options.Setup(o => o.Get(schemeName)).Returns(new TrustedNetworkOptions());
            var logger = new Mock<ILoggerFactory>();
            var encoder = new Mock<UrlEncoder>();
            var validator = new Mock<ITrustedNetworkValidator>();
            var handler = new TestTrustedNetworkAuthorizationHandler(options.Object, logger.Object, encoder.Object, validator.Object);
            HttpContext context = new DefaultHttpContext() { User = new ClaimsPrincipal() };
            var scheme = new AuthenticationScheme(schemeName, displayName: null, typeof(TestBasicAuthenticationHandler));
            await handler.InitializeAsync(scheme, context);

            // Act
            var exception = Assert.Throws<AuthorizationException>(() => handler.TestHandleAuthenticateAsync());

            // Assert
            Assert.That(exception, Is.Not.Null);
        }

        [Test(Description = "Если уже авторизован - аутентификация успешна.")]
        public async Task HandleAuthenticateAsync_Authorize_AuthorizationSuccess()
        {
            // Arrange
            var schemeName = TrustedNetworkOptions.Scheme;
            var options = new Mock<IOptionsMonitor<TrustedNetworkOptions>>();
            options.Setup(o => o.Get(schemeName)).Returns(new TrustedNetworkOptions());
            var logger = new Mock<ILoggerFactory>();
            var encoder = new Mock<UrlEncoder>();
            var validator = new Mock<ITrustedNetworkValidator>();
            var handler = new TestTrustedNetworkAuthorizationHandler(options.Object, logger.Object, encoder.Object, validator.Object);
            HttpContext context = new DefaultHttpContext() { User = new ClaimsPrincipal(new ClaimsIdentity(TrustedNetworkOptions.Scheme)) };
            var scheme = new AuthenticationScheme(schemeName, displayName: null, typeof(TestBasicAuthenticationHandler));
            await handler.InitializeAsync(scheme, context);

            // Act
            var result = await handler.TestHandleAuthenticateAsync();

            // Assert
            Assert.That(result.Succeeded, Is.True);
        }

        [Test(Description = "При неверной схеме аутентификации и пустом заголовке аутентификации - аутентификация с ошибкой.")]
        public async Task HandleAuthenticateAsync_AuthorizationHeaderIsEmpty_AuthorizationFail()
        {
            // Arrange
            var schemeName = "SchemeName";
            var options = new Mock<IOptionsMonitor<TrustedNetworkOptions>>();
            options.Setup(o => o.Get(schemeName)).Returns(new TrustedNetworkOptions());
            var logger = new Mock<ILoggerFactory>();
            var encoder = new Mock<UrlEncoder>();
            var validator = new Mock<ITrustedNetworkValidator>();
            var handler = new TestTrustedNetworkAuthorizationHandler(options.Object, logger.Object, encoder.Object, validator.Object);
            HttpContext context = new DefaultHttpContext() { User = new ClaimsPrincipal(new ClaimsIdentity(string.Empty)) };
            var scheme = new AuthenticationScheme(schemeName, displayName: null, typeof(TestBasicAuthenticationHandler));
            await handler.InitializeAsync(scheme, context);

            // Act
            var result = await handler.TestHandleAuthenticateAsync();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Succeeded, Is.False);
                Assert.That(result.Failure?.Message, Is.EqualTo($"Не задан заголовок '{HeaderNames.Authorization}'"));
            });
        }

        [Test(Description = "При неверном заголовке аутентификации - аутентификация с ошибкой.")]
        public async Task HandleAuthenticateAsync_AuthorizationHeaderIsUnexpected_AuthorizationFail()
        {
            // Arrange
            var schemeName = "SchemeName";
            var options = new Mock<IOptionsMonitor<TrustedNetworkOptions>>();
            options.Setup(o => o.Get(schemeName)).Returns(new TrustedNetworkOptions());
            var logger = new Mock<ILoggerFactory>();
            var encoder = new Mock<UrlEncoder>();
            var validator = new Mock<ITrustedNetworkValidator>();
            var handler = new TestTrustedNetworkAuthorizationHandler(options.Object, logger.Object, encoder.Object, validator.Object);
            HttpContext context = new DefaultHttpContext() { User = new ClaimsPrincipal(new ClaimsIdentity(string.Empty)) };
            context.Request.Headers.Append(HeaderNames.Authorization, schemeName);
            var scheme = new AuthenticationScheme(schemeName, displayName: null, typeof(TestBasicAuthenticationHandler));
            await handler.InitializeAsync(scheme, context);

            // Act
            var result = await handler.TestHandleAuthenticateAsync();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Succeeded, Is.False);
                Assert.That(result.Failure?.Message, Is.EqualTo("Задана неподдерживаемая схема аутентификации"));
            });
        }

        [Test(Description = "При неверном заголовке аутентификации - аутентификация с ошибкой.")]
        public async Task HandleAuthenticateAsync_AuthorizationRoleIsEmpty_AuthorizationFail()
        {
            // Arrange
            var schemeName = "SchemeName";
            var options = new Mock<IOptionsMonitor<TrustedNetworkOptions>>();
            options.Setup(o => o.Get(schemeName)).Returns(new TrustedNetworkOptions());
            var logger = new Mock<ILoggerFactory>();
            var encoder = new Mock<UrlEncoder>();
            var validator = new Mock<ITrustedNetworkValidator>();
            var handler = new TestTrustedNetworkAuthorizationHandler(options.Object, logger.Object, encoder.Object, validator.Object);
            HttpContext context = new DefaultHttpContext() { User = new ClaimsPrincipal(new ClaimsIdentity(string.Empty)) };
            context.Request.Headers.Append(HeaderNames.Authorization, $"{TrustedNetworkOptions.Scheme} ");
            var scheme = new AuthenticationScheme(schemeName, displayName: null, typeof(TestBasicAuthenticationHandler));
            await handler.InitializeAsync(scheme, context);

            // Act
            var result = await handler.TestHandleAuthenticateAsync();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Succeeded, Is.False);
                Assert.That(result.Failure?.Message, Is.EqualTo("Не задана роль пользователя, запросившего аутентификацию"));
            });
        }

        [Test(Description = "Если валидация IP-адреса прошла с ошибкой, то аутентификация с ошибкой.")]
        public async Task HandleAuthenticateAsync_ValidationIpAddressFailed_AuthorizationFail()
        {
            // Arrange
            var schemeName = "SchemeName";
            var options = new Mock<IOptionsMonitor<TrustedNetworkOptions>>();
            options.Setup(o => o.Get(schemeName)).Returns(new TrustedNetworkOptions());
            var logger = new Mock<ILoggerFactory>();
            logger.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(new Mock<ILogger>().Object);
            var encoder = new Mock<UrlEncoder>();
            var validator = new Mock<ITrustedNetworkValidator>();
            var trustedNetworkValidationResult = TrustedNetworkValidationResult.Error(new Mock<IPAddress>(1111).Object, ["ошибка"]);
            validator.Setup(v => v.Validate(It.IsAny<HttpRequest>())).Returns(trustedNetworkValidationResult);
            var handler = new TestTrustedNetworkAuthorizationHandler(options.Object, logger.Object, encoder.Object, validator.Object);
            HttpContext context = new DefaultHttpContext() { User = new ClaimsPrincipal(new ClaimsIdentity(string.Empty)) };
            context.Request.Headers.Append(HeaderNames.Authorization, $"{TrustedNetworkOptions.Scheme} System");
            var scheme = new AuthenticationScheme(schemeName, displayName: null, typeof(TestBasicAuthenticationHandler));
            await handler.InitializeAsync(scheme, context);

            // Act
            var result = await handler.TestHandleAuthenticateAsync();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Succeeded, Is.False);
                Assert.That(result.Failure?.Message, Is.EqualTo("Аутентификация отклонена. Сеть не входит в перечень доверенных."));
            });
        }

        [Test(Description = "Если валидация IP-адреса прошла успешно, то аутентификация успешна.")]
        public async Task HandleAuthenticateAsync_ValidationIpAddressSuccess_AuthorizationSuccess()
        {
            // Arrange
            var schemeName = "SchemeName";
            var options = new Mock<IOptionsMonitor<TrustedNetworkOptions>>();
            options.Setup(o => o.Get(schemeName)).Returns(new TrustedNetworkOptions());
            var logger = new Mock<ILoggerFactory>();
            var encoder = new Mock<UrlEncoder>();
            var validator = new Mock<ITrustedNetworkValidator>();
            var trustedNetworkValidationResult = TrustedNetworkValidationResult.Success(new Mock<IPAddress>(1111).Object);
            validator.Setup(v => v.Validate(It.IsAny<HttpRequest>())).Returns(trustedNetworkValidationResult);
            var handler = new TestTrustedNetworkAuthorizationHandler(options.Object, logger.Object, encoder.Object, validator.Object);
            HttpContext context = new DefaultHttpContext() { User = new ClaimsPrincipal(new ClaimsIdentity(string.Empty)) };
            context.Request.Headers.Append(HeaderNames.Authorization, $"{TrustedNetworkOptions.Scheme} System");
            var scheme = new AuthenticationScheme(schemeName, displayName: null, typeof(TestBasicAuthenticationHandler));
            await handler.InitializeAsync(scheme, context);

            // Act
            var result = await handler.TestHandleAuthenticateAsync();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Succeeded, Is.True);
            });
        }

        internal class TestTrustedNetworkAuthorizationHandler : TrustedNetworkAuthorizationHandler
        {
            public TestTrustedNetworkAuthorizationHandler(
                IOptionsMonitor<TrustedNetworkOptions> options, ILoggerFactory logger, UrlEncoder encoder, ITrustedNetworkValidator validator)
                : base(options, logger, encoder, validator)
            {
            }

            public Task<AuthenticateResult> TestHandleAuthenticateAsync()
            {
                return base.HandleAuthenticateAsync();
            }
        }

        internal class TestBasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
        {
            public TestBasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, 
                ILoggerFactory logger, UrlEncoder encoder)
                : base(options, logger, encoder)
            {
            }

            protected override Task<AuthenticateResult> HandleAuthenticateAsync()
            {
                var ticket = new AuthenticationTicket(new ClaimsPrincipal(), Scheme.Name);

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
        }
    }
}
