using Meta.Common.Infrastructures.DataAccess.Configurations;
using Meta.Common.Infrastructures.DataAccess.Session;
using Meta.Common.Test.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Meta.Common.Tests.UnitTests.Infrastructure.DataAccess.Session
{
    internal class Context(DbContextOptions<Context> options) : DbContext(options)
    {
    }

    internal class Configurator : IDbContextOptionsConfigurator<Context>
    {
        public void Configure(DbContextOptionsBuilder<Context> options)
        {
        }
    }

    /// <summary>
    /// Тесты на сессию при работе с данными через EF Core,
    /// </summary>
    internal class EntityFrameworkDataSessionTests : BaseMemoryDatabaseTests<Context, Configurator>
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

        [Test(Description = "При создании сессии, сессия не может быть неопределенной")]
        public void Create_EntityFrameworkDataSession_SessionIsNotNull()
        {
            // Arrange
            var factory = _serviceProvider.GetRequiredService<IDataSessionFactory>();

            // Act
            var session = factory.Create();

            // Assert
            Assert.That(session, Is.Not.Null);
        }

        [Test(Description = "При отсутствии текущей транзакции, возвращается что активных транзакций нет")]
        public void HasActiveTransaction_CurrentTransactionIsNull_HasActiveTransactionFalse()
        {
            // Arrange
            var factory = _serviceProvider.GetRequiredService<IDataSessionFactory>();
            var session = factory.Create();

            // Act
            var hasActiveTransaction = session.HasActiveTransaction();

            // Assert
            Assert.That(hasActiveTransaction, Is.False);
        }

        [Test(Description = "При запуске транзакций не должно быть исключений")]
        public void BeginTransaction_Valid_NoException()
        {
            // Arrange
            var factory = _serviceProvider.GetRequiredService<IDataSessionFactory>();
            var session = factory.Create();

            // Act
            // Assert
            Assert.DoesNotThrow(session.BeginTransaction);
        }

        [Test(Description = "При запуске транзакций не должно быть исключений")]
        public void BeginTransactionAsync_Valid_NoException()
        {
            // Arrange
            var factory = _serviceProvider.GetRequiredService<IDataSessionFactory>();
            var session = factory.Create();

            // Act
            // Assert
            Assert.DoesNotThrowAsync(() =>
                session.BeginTransactionAsync(CancellationToken.None));
        }

        [Test(Description = "При коммите транзакций не должно быть исключений")]
        public void CommitTransaction_Valid_NoException()
        {
            // Arrange
            var factory = _serviceProvider.GetRequiredService<IDataSessionFactory>();
            var session = factory.Create();

            // Act
            // Assert
            Assert.DoesNotThrow(session.CommitTransaction);
        }

        [Test(Description = "При откате транзакций не должно быть исключений")]
        public void RollbackTransaction_Valid_NoException()
        {
            // Arrange
            var factory = _serviceProvider.GetRequiredService<IDataSessionFactory>();
            var session = factory.Create();

            // Act
            // Assert
            Assert.DoesNotThrow(() =>
                session.RollbackTransaction());
        }

        [Test(Description = "При откате транзакций не должно быть исключений")]
        public void Dispose_Valid_NoException()
        {
            // Arrange
            var factory = _serviceProvider.GetRequiredService<IDataSessionFactory>();
            var session = factory.Create();

            // Act
            // Assert
            Assert.DoesNotThrow(session.Dispose);
        }
    }
}
