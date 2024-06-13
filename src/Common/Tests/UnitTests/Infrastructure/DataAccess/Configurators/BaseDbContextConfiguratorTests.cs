using Meta.Common.Domain;
using Meta.Common.Infrastructures.DataAccess.Configurators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace Meta.Common.Tests.UnitTests.Infrastructure.DataAccess.Configurators
{
    /// <summary>
    /// Тесты общего конфигуратора БД.
    /// </summary>
    internal class BaseDbContextConfiguratorTests
    {
        internal class Configurator(IConfiguration configuration, ILoggerFactory loggerFactory) 
            : BaseDbContextConfigurator<Context>(configuration, loggerFactory)
        {
            protected override string ConnectionStringName => string.Empty;
        }

        internal class ValidConfigurator(IConfiguration configuration, ILoggerFactory loggerFactory)
            : BaseDbContextConfigurator<Context>(configuration, loggerFactory)
        {
            protected override string ConnectionStringName => "DB";
        }

        public class Context(DbContextOptions<Context> options) : DbContext(options)
        {
            /// <inheritdoc/>
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<TestEntity>();
            }
        }

        public class TestEntity : Entity<int>
        {
            public string? Key { get; set; }
        }

        [Test(Description = "Если connectionString пустой, то выбрасывается исключение.")]
        public void Configure_ConnectionStringIsEmpty_InvalidOperationException()
        {
            // Arrange
            var keys = new Dictionary<string, string?>();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(keys)
                .Build();
            var loggerFactory = new Mock<ILoggerFactory>();
            var configurator = new Configurator(configuration, loggerFactory.Object);
            var options = new DbContextOptionsBuilder<Context>();

            // Act
            var exception = Assert.Throws<InvalidOperationException>(() => 
                                configurator.Configure(options));

            // Assert
            Assert.That(exception, Is.Not.Null);
        }

        [Test(Description = "Если connectionString пустой, то выбрасывается исключение.")]
        public void Configure_Valid_InvalidOperationException()
        {
            // Arrange
            var keys = new Dictionary<string, string?> 
            { 
                { "ConnectionStrings", string.Empty },
                { "ConnectionStrings:DB", "db" },
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(keys)
                .Build();
            var loggerFactory = new Mock<ILoggerFactory>();
            var configurator = new ValidConfigurator(configuration, loggerFactory.Object);
            var options = new DbContextOptionsBuilder<Context>();

            // Act
            // Assert
            Assert.DoesNotThrow(() => configurator.Configure(options));
        }
    }
}
