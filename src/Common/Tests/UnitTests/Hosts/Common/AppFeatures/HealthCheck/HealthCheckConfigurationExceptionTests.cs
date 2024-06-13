using Meta.Common.Contracts.Exceptions.Common;
using Meta.Common.Hosts.Features.AppFeatures.HealthCheck.Base;

namespace Meta.Common.Test.UnitTests.Hosts.Common.AppFeatures.HealthCheck
{
    /// <summary>
    /// Тесты исключения, бросаемого при конфигурировании HealthCheck'ов.
    /// </summary>
    internal class HealthCheckConfigurationExceptionTests
    {
        [Test(Description = "Переданный текст в исключение должен сохраняться.")]
        public void HealthCheckConfigurationException_Message_MessageEquals()
        {
            // Act
            var exception = new HealthCheckConfigurationException("Сообщение");

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Сообщение"));
        }

        [Test(Description = "Переданное внутренее исключение должно сохраняться.")]
        public void HealthCheckConfigurationException_InnerException_InnerExceptionTypeEquals()
        {
            // Act
            var innerException = new NotImplementedException();
            var exception = new HealthCheckConfigurationException("Сообщение", innerException);

            // Assert
            Assert.That(exception.InnerException?.GetType(), Is.EqualTo(typeof(NotImplementedException)));
        }
    }
}
