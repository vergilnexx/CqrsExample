using Meta.Common.Hosts.RabbitMq.Options;

namespace Meta.Common.Test.UnitTests.Hosts.Common.RabbitMq.Options
{
    //Тесты настроек RabbitMq.
    internal class RabbitMqOptionsTests
    {
        [Test(Description = "При настройке интервал периодической отправки сигнала, значение должно сохраняться")]
        public void Heartbeat_Valid_ValueSaved()
        {
            // Arrange
            var rabbitMqOptions = new RabbitMqOptions()
            {
                Hosts = "localhost",
                VirtualHost = "localhost",
                Port = "567",
                UserName = "username",
                Password = "password",
            };

            // Act
            rabbitMqOptions.Heartbeat = TimeSpan.FromSeconds(1);

            // Assert
            Assert.That(rabbitMqOptions.Heartbeat, Is.EqualTo(TimeSpan.FromSeconds(1)));
        }

        [Test(Description = "При настройке интервал периодической отправки сигнала, значение должно сохраняться")]
        public void Heartbeat_Negative_ThrowException()
        {
            // Arrange
            var rabbitMqOptions = new RabbitMqOptions()
            {
                Hosts = "localhost",
                VirtualHost = "localhost",
                Port = "567",
                UserName = "username",
                Password = "password",
            };

            // Act
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                rabbitMqOptions.Heartbeat = TimeSpan.FromSeconds(-1);
            });

            // Assert
            Assert.That(exception?.Message.Contains("Heartbeat"), Is.True);
        }
    }
}
