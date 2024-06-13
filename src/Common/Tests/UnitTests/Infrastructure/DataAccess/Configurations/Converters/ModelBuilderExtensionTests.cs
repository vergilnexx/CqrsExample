using Meta.Common.Domain;
using Meta.Common.Infrastructures.DataAccess.Configurations;
using Meta.Common.Test.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Meta.Common.Tests.UnitTests.Infrastructure.DataAccess.Configurations.Converters
{
    internal class Context(DbContextOptions<Context> options) : DbContext(options)
    {
        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestEntity>();

            ModelBuilderExtension.SetDefaultDateTimeKind(modelBuilder, DateTimeKind.Utc);
        }
    }

    internal class Configurator : IDbContextOptionsConfigurator<Context>
    {
        public void Configure(DbContextOptionsBuilder<Context> options)
        {
        }
    }

    internal class TestEntity : Entity<int>
    {
        public DateTime? Key { get; set; }
    }

    /// <summary>
    /// Тесты расширения для сборщика моделей.
    /// </summary>
    internal class ModelBuilderExtensionTests : BaseMemoryDatabaseTests<Context, Configurator>
    {
        [SetUp]
        public void Setup()
        {
            Init();
        }

        [TearDown]
        public void TearDown()
        {
            Cleanup();
        }

        [Test(Description = "Если моделей нет, то вызов не выбрасывает исключения")]
        public void ModelBuilderExtension_ModelIsEmpty_NoException()
        {
            // Arrange
            var modelBuilder = new ModelBuilder();

            // Act
            // Assert
            Assert.DoesNotThrow(() =>
                modelBuilder.SetDefaultDateTimeKind(DateTimeKind.Utc));
        }
    }
}
