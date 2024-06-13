using MassTransit;
using Meta.Common.Contracts.Abstract;
using Meta.Common.Contracts.Events;
using Meta.Common.Hosts.RabbitMq.Exceptions;
using Meta.Common.Hosts.RabbitMq.Registrar.Client;
using Moq;

namespace Meta.Common.Test.UnitTests.Hosts.Common.RabbitMq.Registrar.Client
{
    /// <summary>
    /// Тесты клиентов rabbitMq.
    /// </summary>
    internal class RabbitMqClientTests
    {
        internal interface ITestMessage : IEvent { }

        /// <summary>
        /// Тестовое сообщение
        /// </summary>
        internal class TestMessage : Meta.Common.Contracts.Abstract.Event, ITestMessage
        {
            /// <summary>
            /// Текст
            /// </summary>
            public string? Text { get; set; }
        }

        [Test(Description = "При публикации необределенного сообщения в шину, должно выбрасываться исключение")]
        public void PublishAsync_MessageIsNull_ArgumentNullException()
        {
            // Arrange
            var control = new Mock<IBusControl>();
            var client = new RabbitMqClient(control.Object);
            TestMessage? message = null;

            // Act
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await client.PublishAsync(message, CancellationToken.None);
            });

            // Assert
            Assert.That(exception?.Message.Contains("message"), Is.True);
        }

        [Test(Description = "При публикации сообщения в шину, метод публикации вызывается один раз")]
        public async Task PublishAsync_ValidMessage_PublishWillBeCalledOnce()
        {
            // Arrange
            var control = new Mock<IBusControl>();
            var client = new RabbitMqClient(control.Object);
            var message = new TestMessage();

            // Act
            await client.PublishAsync(message, CancellationToken.None);

            // Assert
            control.Verify(c => c.Publish(message, typeof(ITestMessage), CancellationToken.None), Times.Once);
        }

        [Test(Description = "При публикации выбрасывается исключение, новое исключение должно содержать внутреннее")]
        public void PublishAsync_PublishThrowException_ExceptionContainsInnerException()
        {
            // Arrange
            var control = new Mock<IBusControl>();
            control.Setup(c => c.Publish(It.IsAny<TestMessage>(), typeof(ITestMessage), CancellationToken.None)).ThrowsAsync(new NotImplementedException());
            var client = new RabbitMqClient(control.Object);
            var message = new TestMessage();

            // Act
            var exception = Assert.ThrowsAsync<RabbitMqServiceException>(async () =>
            {
                await client.PublishAsync(message, CancellationToken.None);
            });

            // Assert
            Assert.That(exception?.InnerException, Is.TypeOf<NotImplementedException>());
        }
    }
}
