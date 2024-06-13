using Meta.Common.Contracts.Options;
using Meta.Common.Hosts.Features.AppFeatures.Authorization.Instances.TrustedNetwork;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;

namespace Meta.Common.Tests.UnitTests.Hosts.Api.Features.Authorization
{
    /// <summary>
    /// Тесты валидатора доверенных сетей.
    /// </summary>
    internal class TrustedNetworkValidatorTests
    {
        [Test(Description = "Если при валидации список доверенных сетей пустой, то валидация происходит с ошибкой")]
        public void Validate_NetworksIsEmpty_ValidationIsFailed()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var request =  httpContext.Request;
            var options = new Mock<IOptions<TrustedNetworkOptions>>();
            options.Setup(o => o.Value).Returns(new TrustedNetworkOptions());
            var validator = new TrustedNetworkValidator(options.Object);

            // Act
            var result = validator.Validate(request);

            // Assert
            Assert.That(result.Errors, Is.Not.Empty);
        }

        [Test(Description = "Если при валидации IP адрес входит в список доверенных сетей, то валидация успешна")]
        public void Validate_IpIsNull_ValidationIsFailed()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var request = httpContext.Request;
            var options = new Mock<IOptions<TrustedNetworkOptions>>();
            options.Setup(o => o.Value).Returns(new TrustedNetworkOptions() { TrustedNetworks = "127.0.0.0/8" });
            var validator = new TrustedNetworkValidator(options.Object);

            // Act
            var result = validator.Validate(request);

            // Assert
            Assert.That(result.Errors, Is.Not.Empty);
        }

        [Test(Description = "Если при валидации IP адрес(v4) входит в список доверенных сетей, то валидация успешна")]
        public void Validate_NetworksV4IsSame_ValidationIsSucceded()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-Original-Forwarded-For"] = "127.0.0.1";
            var request = httpContext.Request;
            var options = new Mock<IOptions<TrustedNetworkOptions>>();
            options.Setup(o => o.Value).Returns(new TrustedNetworkOptions() { TrustedNetworks = "127.0.0.0/8" });
            var validator = new TrustedNetworkValidator(options.Object);

            // Act
            var result = validator.Validate(request);

            // Assert
            Assert.That(result.Errors, Is.Empty);
        }

        [Test(Description = "Если при валидации IP адрес(v6) входит в список доверенных сетей, то валидация успешна")]
        public void Validate_NetworksV6IsSame_ValidationIsSucceded()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-Original-Forwarded-For"] = "::1";
            var request = httpContext.Request;
            var options = new Mock<IOptions<TrustedNetworkOptions>>();
            options.Setup(o => o.Value).Returns(new TrustedNetworkOptions() { TrustedNetworks = "::1/128" });
            var validator = new TrustedNetworkValidator(options.Object);

            // Act
            var result = validator.Validate(request);

            // Assert
            Assert.That(result.Errors, Is.Empty);
        }
    }
}
