using Meta.Common.Hosts.Features.AppFeatures.Base;

namespace Meta.Common.Test.UnitTests.Hosts.Common.AppFeatures.Base
{
    /// <summary>
    /// Тесты на исключения, бросаемое при конфигурировании функциональности.
    /// </summary>
    internal class FeatureConfigurationExceptionTests
    {
        [Test(Description = "Переданный текст в исключение, должен сохраняться.")]
        public void NotFoundException_Message_MessageEquals()
        {
            // Act
            var exception = new FeatureConfigurationException("Сообщение");

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Сообщение"));
        }
    }
}
