using Meta.Common.Hosts.RabbitMq.Options;
using Meta.Common.Hosts.RabbitMq.Registrar.Client;
using Microsoft.Extensions.Options;
using Moq;

namespace Meta.Common.Test.UnitTests.Hosts.Common.RabbitMq.Registrar.Client
{
    /// <summary>
    /// Тесты фабрики клиента RabbitMq.
    /// </summary>
    internal class RabbitMqClientFactoryTests
    {
        [Test(Description = "При валидных настройках, фабрика должна возвращать рабочего клиента")]
        public void GetClient_ValidOptions_ClientIsNotNull()
        {
            // Arrange
            var options = new Mock<IOptions<RabbitMqOptions>>();
            var rabbitMqOptions = new RabbitMqOptions()
            {
                Hosts = "localhost",
                UserName = "username",
                Password = "password",
                VirtualHost = "localhost",
                Port = "5672",
                Heartbeat = TimeSpan.FromSeconds(1)
            };
            options.SetupGet(o => o.Value).Returns(rabbitMqOptions);
            var factory = new RabbitMqClientFactory(options.Object);

            // Act
            var client = factory.GetClient();

            // Assert
            Assert.That(client, Is.Not.Null);
        }

        [Test(Description = "При валидных настройках с несколькими хостами, фабрика должна возвращать рабочего клиента")]
        public void GetClient_FewHosts_ClientIsNotNull()
        {
            // Arrange
            var options = new Mock<IOptions<RabbitMqOptions>>();
            var rabbitMqOptions = new RabbitMqOptions()
            {
                Hosts = "localhost1,localhost2",
                UserName = "username",
                Password = "password",
                VirtualHost = "localhost",
                Port = "5672",
            };
            options.SetupGet(o => o.Value).Returns(rabbitMqOptions);
            var factory = new RabbitMqClientFactory(options.Object);

            // Act
            var client = factory.GetClient();

            // Assert
            Assert.That(client, Is.Not.Null);
        }
    }
}
