using Meta.Common.Contracts.Events;
using Meta.Common.Hosts.RabbitMq.Options;
using Meta.Common.Hosts.RabbitMq.Registrar.Service;
using Microsoft.Extensions.Options;
using Moq;

namespace Meta.Common.Test.UnitTests.Hosts.Common.RabbitMq.Registrar.Service
{
    /// <summary>
    /// Тестовый обработчик
    /// </summary>
    public class Consumer : INamedConsumer
    {
        public string QueueName => "Consumer";
    }

    /// <summary>
    /// Другой тестовый обработчик
    /// </summary>
    public class SecondConsumer : INamedConsumer
    {
        public string QueueName => "SecondConsumer";
    }

    /// <summary>
    /// Тесты фабрики сервисов RabbitMq.
    /// </summary>
    internal class RabbitMqServiceFactoryTests
    {
        [Test(Description = "Если пустой список обработчиков сообщений, то сервис инициализируется без проблем")]
        public void GetService_ConsumersIsEmpty_ServiceIsNotNull()
        {
            // Arrange
            var options = new Mock<IOptions<RabbitMqOptions>>();
            var rabbitMqOptions = new RabbitMqOptions()
            {
                Hosts = ["localhost"],
                VirtualHost = "localhost",
                Port = "567",
                UserName = "username",
                Password = "password",
            };
            options.Setup(o => o.Value).Returns(rabbitMqOptions);
            var consumers = Array.Empty<INamedConsumer>();

            var factory = new RabbitMqServiceFactory(options.Object, consumers);

            // Act
            var service = factory.GetService();

            // Assert
            Assert.That(service, Is.Not.Null);
        }

        [Test(Description = "Если есть список обработчиков сообщений, то сервис инициализируется без проблем")]
        public void GetService_FewHosts_ServiceIsNotNull()
        {
            // Arrange
            var options = new Mock<IOptions<RabbitMqOptions>>();
            var rabbitMqOptions = new RabbitMqOptions()
            {
                Hosts = ["localhost1", "localhost2"],
                VirtualHost = "localhost",
                Port = "567",
                UserName = "username",
                Password = "password",
            };
            options.Setup(o => o.Value).Returns(rabbitMqOptions);
            var consumer = new Consumer();
            var consumers = new INamedConsumer[] { consumer };

            var factory = new RabbitMqServiceFactory(options.Object, consumers);

            // Act
            var service = factory.GetService();

            // Assert
            Assert.That(service, Is.Not.Null);
        }

        [Test(Description = "Если есть список обработчиков сообщений, то каждый должен быть инициализирован")]
        public void GetService_ValidConsumers_ConsumersWereInit()
        {
            // Arrange
            var options = new Mock<IOptions<RabbitMqOptions>>();
            var rabbitMqOptions = new RabbitMqOptions()
            {
                Hosts = ["localhost"],
                VirtualHost = "localhost",
                Port = "567",
                UserName = "username",
                Password = "password",
            };
            options.Setup(o => o.Value).Returns(rabbitMqOptions);
            var consumer1 = new Consumer();
            var consumer2 = new SecondConsumer();
            var consumers = new INamedConsumer[] { consumer1, consumer2 };

            var factory = new RabbitMqServiceFactory(options.Object, consumers);

            // Act
            var service = factory.GetService();

            // Assert
            Assert.That(service, Is.Not.Null);
        }
    }
}
