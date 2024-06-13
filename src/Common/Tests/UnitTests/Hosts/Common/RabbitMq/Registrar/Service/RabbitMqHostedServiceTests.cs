using MassTransit;
using Meta.Common.Hosts.RabbitMq.Exceptions;
using Meta.Common.Hosts.RabbitMq.Registrar.Service;
using Moq;

namespace Meta.Common.Test.UnitTests.Hosts.Common.RabbitMq.Registrar.Service
{
    /// <summary>
    /// Тесты сервиса RabbitMq.
    /// </summary>
    internal class RabbitMqHostedServiceTests
    {
        [Test(Description = "При запуске, запуск должен осуществляться один раз.")]
        public async Task StartAsync_Valid_StartWillBeCalledOnce()
        {
            // Arrange
            var control = new Mock<IBusControl>();
            var service = new RabbitMqHostedService(control.Object);

            // Act
            await service.StartAsync(CancellationToken.None);

            // Assert
            control.Verify(c => c.StartAsync(CancellationToken.None), Times.Once);
        }

        [Test(Description = "Если при запуске выбросило исключение, то новое исключение должно содержать внутреннее.")]
        public void StartAsync_StartThrowsException_ExceptionContainsInnerException()
        {
            // Arrange
            var control = new Mock<IBusControl>();
            control.Setup(c => c.StartAsync(CancellationToken.None)).ThrowsAsync(new NotImplementedException());
            var service = new RabbitMqHostedService(control.Object);

            // Act
            var exception = Assert.ThrowsAsync<RabbitMqServiceException>(async () =>
            {
                await service.StartAsync(CancellationToken.None);
            });

            // Assert
            Assert.That(exception?.InnerException, Is.TypeOf<NotImplementedException>());
        }

        [Test(Description = "При остановке, остановка должен осуществляться один раз.")]
        public async Task StopAsync_Valid_StartWillBeCalledOnce()
        {
            // Arrange
            var control = new Mock<IBusControl>();
            var service = new RabbitMqHostedService(control.Object);

            // Act
            await service.StopAsync(CancellationToken.None);

            // Assert
            control.Verify(c => c.StopAsync(CancellationToken.None), Times.Once);
        }

        [Test(Description = "Если при остановке выбросило исключение, то новое исключение должно содержать внутреннее.")]
        public void StopAsync_StartThrowsException_ExceptionContainsInnerException()
        {
            // Arrange
            var control = new Mock<IBusControl>();
            control.Setup(c => c.StopAsync(CancellationToken.None)).ThrowsAsync(new NotImplementedException());
            var service = new RabbitMqHostedService(control.Object);

            // Act
            var exception = Assert.ThrowsAsync<RabbitMqServiceException>(async () =>
            {
                await service.StopAsync(CancellationToken.None);
            });

            // Assert
            Assert.That(exception?.InnerException, Is.TypeOf<NotImplementedException>());
        }
    }
}
