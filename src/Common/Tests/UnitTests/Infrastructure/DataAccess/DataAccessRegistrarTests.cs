using MassTransit;
using Meta.Common.Infrastructures.DataAccess.Configurations;
using Meta.Common.Infrastructures.DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Meta.Common.Tests.UnitTests.Infrastructure.DataAccess
{
    /// <summary>
    /// Тесты регистрации доступа к данным.
    /// </summary>
    internal class DataAccessRegistrarTests
    {
        public class Context(DbContextOptions<Context> options) : DbContext(options)
        {
        }

        public class Configurator : IDbContextOptionsConfigurator<Context>
        {
            public bool IsCalled { get; set; }

            public void Configure(DbContextOptionsBuilder<Context> options)
            {
                IsCalled = true;
            }
        }

        [Test(Description = "При добавлении доступа к данным не должно быть исключений")]
        public void AddDataAccess_Valid_NoException()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddDataAccess<Context, Configurator>();

            // Assert
            Assert.That(services, Is.Not.Empty);
        }

        [Test(Description = "При конфигурировании доступа к данным вызывается конфигурирование")]
        public void Configure_Valid_ConfigureIsCalled()
        {
            // Arrange
            var configurator = new Configurator();
            var services = new ServiceCollection();
            services.AddSingleton<IDbContextOptionsConfigurator<Context>>(configurator);
            var provider = services.BuildServiceProvider();
            var options = new DbContextOptionsBuilder<Context>();

            // Act
            DataAccessRegistrar.Configure<Context>(provider, options);

            // Assert
            Assert.That(configurator.IsCalled, Is.True);
        }
    }
}
