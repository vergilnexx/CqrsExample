using FluentValidation;
using Meta.Common.Contracts.Exceptions.Common;
using Meta.Common.Hosts.Api.Features.AppFeatures.ExceptionHandler;
using Meta.Common.Hosts.Features.AppFeatures.Authorization.Instances.TrustedNetwork;
using Meta.Common.Hosts.Features.AppFeatures.Authorization.Instances.TrustedNetwork.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;

namespace Meta.Common.Tests.UnitTests.Hosts.Api.Features.ExceptionHandler
{
    /// <summary>
    /// Тесты функциональности обработки исключений.
    /// </summary>
    internal class ExceptionHandlingMiddlewareTests
    {
        [Test(Description = "Если контекст не задан - выбрасывается исключение")]
        public void UseFeature_ContextIsNull_ArgumentNullException()
        {
            // Arrange
            HttpContext context = new DefaultHttpContext();
            RequestDelegate next = (HttpContext hc) => Task.CompletedTask;
            var middleware = new ExceptionHandlingMiddleware(next);
            var environment = new Mock<IHostEnvironment>();
            var provider = new Mock<IServiceProvider>();

            // Act
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await middleware.Invoke(context: null, logger: null, environment.Object, provider.Object);
            });

            // Assert
            Assert.That(exception?.ParamName, Is.EqualTo("context"));
        }

        [Test(Description = "Вызов middleware, должно происходить без выбрасывания исключения")]
        public void UseFeature_Valid_NoException()
        {
            // Arrange
            HttpContext context = new DefaultHttpContext();
            RequestDelegate next = (HttpContext hc) => Task.CompletedTask;
            var middleware = new ExceptionHandlingMiddleware(next);
            var environment = new Mock<IHostEnvironment>();
            var provider = new Mock<IServiceProvider>();

            // Act
            // Assert
            Assert.DoesNotThrowAsync(async () =>
            {
                await middleware.Invoke(context, logger: null, environment.Object, provider.Object);
            });
        }

        [Test(Description = "Если падает исключение неавторизованности, то код ответа 403")]
        public async Task UseFeature_ThrowUnauthorizedAccessException_ForbiddenResponseCode()
        {
            // Arrange
            HttpContext context = new DefaultHttpContext();
            RequestDelegate next = (HttpContext hc) => throw new UnauthorizedAccessException();
            var middleware = new ExceptionHandlingMiddleware(next);
            var environment = new Mock<IHostEnvironment>();
            var provider = new Mock<IServiceProvider>();
            var logger = new Mock<ILogger<ExceptionHandlingMiddleware>>();

            // Act
            await middleware.Invoke(context, logger.Object, environment.Object, provider.Object);

            // Assert
            Assert.That(context.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test(Description = "Если падает исключение валидации, то код ответа 400")]
        public async Task UseFeature_ThrowValidationException_BadRequestResponseCode()
        {
            // Arrange
            HttpContext context = new DefaultHttpContext();
            RequestDelegate next = (HttpContext hc) => throw new ValidationException("error");
            var middleware = new ExceptionHandlingMiddleware(next);
            var environment = new Mock<IHostEnvironment>();
            var provider = new Mock<IServiceProvider>();
            var logger = new Mock<ILogger<ExceptionHandlingMiddleware>>();

            // Act
            await middleware.Invoke(context, logger.Object, environment.Object, provider.Object);

            // Assert
            Assert.That(context.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
        }

        [Test(Description = "Если падает исключение таймаута, то код ответа 408")]
        public async Task UseFeature_ThrowOperationCanceledException_RequestTimeoutResponseCode()
        {
            // Arrange
            HttpContext context = new DefaultHttpContext();
            RequestDelegate next = (HttpContext hc) => throw new OperationCanceledException();
            var middleware = new ExceptionHandlingMiddleware(next);
            var environment = new Mock<IHostEnvironment>();
            var provider = new Mock<IServiceProvider>();
            var logger = new Mock<ILogger<ExceptionHandlingMiddleware>>();

            // Act
            await middleware.Invoke(context, logger.Object, environment.Object, provider.Object);

            // Assert
            Assert.That(context.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.RequestTimeout));
        }

        [Test(Description = "Если падает необрабатываемое исключение, то код ответа 500")]
        public async Task UseFeature_ThrowNotImplementedException_InternalServerErrorResponseCode()
        {
            // Arrange
            HttpContext context = new DefaultHttpContext();
            RequestDelegate next = (HttpContext hc) => throw new NotImplementedException();
            var middleware = new ExceptionHandlingMiddleware(next);
            var environment = new Mock<IHostEnvironment>();
            var provider = new Mock<IServiceProvider>();
            var logger = new Mock<ILogger<ExceptionHandlingMiddleware>>();

            // Act
            await middleware.Invoke(context, logger.Object, environment.Object, provider.Object);

            // Assert
            Assert.That(context.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));
        }

        [Test(Description = "При формировании API ошибки, если при валидации произошла ошибка, возвращается ответ с ошибкой")]
        public void CreateApiError_TrustedNetworkValidationResultError_ApiError()
        {
            // Arrange
            HttpContext context = new DefaultHttpContext();
            var environment = new HostingEnvironment() { EnvironmentName = "Production" };
            var provider = new Mock<IServiceProvider>();
            var validator = new Mock<ITrustedNetworkValidator>();
            provider.Setup(p => p.GetService(typeof(ITrustedNetworkValidator))).Returns(validator.Object);
            var trustedNetworkValidationResult = TrustedNetworkValidationResult.Error(new Mock<IPAddress>(1111).Object, ["ошибка"]);
            validator.Setup(v => v.Validate(It.IsAny<HttpRequest>())).Returns(trustedNetworkValidationResult);

            // Act
            var error = ExceptionHandlingMiddleware.CreateApiError(context, new NotImplementedException("исключение"), environment, provider.Object);

            // Assert
            Assert.That(error.Message.Contains("исключение"), Is.True);
        }

        [Test(Description = "При формировании API ошибки, если при валидации произошла ошибка, возвращается ответ с ошибкой")]
        public void CreateApiError_DevelopmentEnvironment_ApiError()
        {
            // Arrange
            HttpContext context = new DefaultHttpContext();
            var environment = new HostingEnvironment() { EnvironmentName = "Development" };
            var provider = new Mock<IServiceProvider>();

            // Act
            var error = ExceptionHandlingMiddleware.CreateApiError(context, new NotImplementedException("исключение"), environment, provider.Object);

            // Assert
            Assert.That(error.Message.Contains("исключение"), Is.True);
        }

        [Test(Description = "При формировании API ошибки, если исключение об отсутствии прав, ошибка должна об этом говорить")]
        public void CreateApiError_UnauthorizedAccessException_ApiError()
        {
            // Arrange
            HttpContext context = new DefaultHttpContext();
            var environment = new HostingEnvironment() { EnvironmentName = "Production" };
            var provider = new Mock<IServiceProvider>();
            var validator = new Mock<ITrustedNetworkValidator>();
            provider.Setup(p => p.GetService(typeof(ITrustedNetworkValidator))).Returns(validator.Object);
            var trustedNetworkValidationResult = TrustedNetworkValidationResult.Success(new Mock<IPAddress>(1111).Object);
            validator.Setup(v => v.Validate(It.IsAny<HttpRequest>())).Returns(trustedNetworkValidationResult);

            // Act
            var error = ExceptionHandlingMiddleware.CreateApiError(context, new UnauthorizedAccessException("исключение"), environment, provider.Object);

            // Assert
            Assert.That(error.Message, Is.EqualTo("Отсутствуют права на выполнение действия"));
        }

        [Test(Description = "При формировании API ошибки, если исключение об отмене операции, ошибка должна об этом говорить")]
        public void CreateApiError_OperationCanceledException_ApiError()
        {
            // Arrange
            HttpContext context = new DefaultHttpContext();
            var environment = new HostingEnvironment() { EnvironmentName = "Production" };
            var provider = new Mock<IServiceProvider>();
            var validator = new Mock<ITrustedNetworkValidator>();
            provider.Setup(p => p.GetService(typeof(ITrustedNetworkValidator))).Returns(validator.Object);
            var trustedNetworkValidationResult = TrustedNetworkValidationResult.Success(new Mock<IPAddress>(1111).Object);
            validator.Setup(v => v.Validate(It.IsAny<HttpRequest>())).Returns(trustedNetworkValidationResult);

            // Act
            var error = ExceptionHandlingMiddleware.CreateApiError(context, new OperationCanceledException("исключение"), environment, provider.Object);

            // Assert
            Assert.That(error.Message, Is.EqualTo("Операция была отменена"));
        }

        [Test(Description = "При формировании API ошибки, если исключение об ошибке в валидации, ошибка должна об этом говорить")]
        public void CreateApiError_ValidationException_ApiError()
        {
            // Arrange
            HttpContext context = new DefaultHttpContext();
            var environment = new HostingEnvironment() { EnvironmentName = "Production" };
            var provider = new Mock<IServiceProvider>();
            var validator = new Mock<ITrustedNetworkValidator>();
            provider.Setup(p => p.GetService(typeof(ITrustedNetworkValidator))).Returns(validator.Object);
            var trustedNetworkValidationResult = TrustedNetworkValidationResult.Success(new Mock<IPAddress>(1111).Object);
            validator.Setup(v => v.Validate(It.IsAny<HttpRequest>())).Returns(trustedNetworkValidationResult);

            // Act
            var error = ExceptionHandlingMiddleware.CreateApiError(context, new ValidationException("исключение"), environment, provider.Object);

            // Assert
            Assert.That(error.Message, Is.EqualTo("Переданы невалидные данные"));
        }

        [Test(Description = "При формировании API ошибки, если читабельное искючение, ошибка должна об этом говорить")]
        public void CreateApiError_ReadableException_ApiError()
        {
            // Arrange
            HttpContext context = new DefaultHttpContext();
            var environment = new HostingEnvironment() { EnvironmentName = "Production" };
            var provider = new Mock<IServiceProvider>();
            var validator = new Mock<ITrustedNetworkValidator>();
            provider.Setup(p => p.GetService(typeof(ITrustedNetworkValidator))).Returns(validator.Object);
            var trustedNetworkValidationResult = TrustedNetworkValidationResult.Success(new Mock<IPAddress>(1111).Object);
            validator.Setup(v => v.Validate(It.IsAny<HttpRequest>())).Returns(trustedNetworkValidationResult);

            // Act
            var error = ExceptionHandlingMiddleware.CreateApiError(context, new ReadableException("исключение"), environment, provider.Object);

            // Assert
            Assert.That(error.Message, Is.EqualTo("исключение"));
        }


        [Test(Description = "При формировании API ошибки, если необрабатываемое искючение, ошибка должна об этом говорить")]
        public void CreateApiError_UnhandledException_ApiError()
        {
            // Arrange
            HttpContext context = new DefaultHttpContext();
            var environment = new HostingEnvironment() { EnvironmentName = "Production" };
            var provider = new Mock<IServiceProvider>();
            var validator = new Mock<ITrustedNetworkValidator>();
            provider.Setup(p => p.GetService(typeof(ITrustedNetworkValidator))).Returns(validator.Object);
            var trustedNetworkValidationResult = TrustedNetworkValidationResult.Success(new Mock<IPAddress>(1111).Object);
            validator.Setup(v => v.Validate(It.IsAny<HttpRequest>())).Returns(trustedNetworkValidationResult);

            // Act
            var error = ExceptionHandlingMiddleware.CreateApiError(context, new NotImplementedException("исключение"), environment, provider.Object);

            // Assert
            Assert.That(error.Message, Is.EqualTo("Произошла непредвиденная ошибка"));
        }
    }
}