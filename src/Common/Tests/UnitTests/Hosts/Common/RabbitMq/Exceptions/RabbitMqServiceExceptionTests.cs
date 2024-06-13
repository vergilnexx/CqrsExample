using Meta.Common.Hosts.RabbitMq.Exceptions;

namespace Meta.Common.Test.UnitTests.Hosts.Common.RabbitMq.Exceptions
{
    /// <summary>
    /// Тесты исключения, выбрасываемоего при работе с шиной очередей.
    /// </summary>
    internal class RabbitMqServiceExceptionTests
    {
        [Test(Description = "Переданный текст в исключение должен сохраняться.")]
        public void RabbitMqServiceException_Message_MessageEquals()
        {
            // Act
            var exception = new RabbitMqServiceException("Сообщение");

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Сообщение"));
        }

        [Test(Description = "Переданное внутренее исключение должно сохраняться.")]
        public void RabbitMqServiceException_InnerException_InnerExceptionTypeEquals()
        {
            // Act
            var innerException = new NotImplementedException();
            var exception = new RabbitMqServiceException("Сообщение", innerException);

            // Assert
            Assert.That(exception.InnerException?.GetType(), Is.EqualTo(typeof(NotImplementedException)));
        }
    }
}
