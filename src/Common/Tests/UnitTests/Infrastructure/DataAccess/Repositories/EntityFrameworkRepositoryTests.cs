using Meta.Common.Domain;
using Meta.Common.Infrastructures.DataAccess.Configurations;
using Meta.Common.Infrastructures.DataAccess.Repositories;
using Meta.Common.Test.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Meta.Common.Tests.UnitTests.Infrastructure.DataAccess.Repositories
{
    internal class Context(DbContextOptions<Context> options) : DbContext(options)
    {
        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestEntity>();
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
        public string? Key { get; set; }
    }

    /// <summary>
    /// Тесты репозитория.
    /// </summary>
    internal class EntityFrameworkRepositoryTests : BaseMemoryDatabaseTests<Context, Configurator>
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

        [Test(Description = "При формировании запроса к данным, запрос не может быть неопределенным")]
        public void AsQueryable_Valid_QueryIsNotNull()
        {
            // Arrange
            var repository = _serviceProvider.GetRequiredService<IRepository<TestEntity>>();

            // Act
            var query = repository.AsQueryable();

            // Assert
            Assert.That(query, Is.Not.Null);
        }

        [Test(Description = "При формализации незаполненных данных, данные должны быть пустыми")]
        public async Task AsQueryable_ToArray_DataIsEmpty()
        {
            // Arrange
            var repository = _serviceProvider.GetRequiredService<IRepository<TestEntity>>();

            // Act
            var result = await repository.AsQueryable().ToArrayAsync(CancellationToken.None);

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test(Description = "После добавления записи, в репозитории должна быть одна запись")]
        public async Task AddAsync_One_RepositoryHasOneEntity()
        {
            // Arrange
            var repository = _serviceProvider.GetRequiredService<IRepository<TestEntity>>();

            // Act
            await repository.AddAsync(new TestEntity(), CancellationToken.None);

            // Assert
            var count = await repository.AsQueryable().CountAsync(CancellationToken.None);
            Assert.That(count, Is.EqualTo(1));
        }

        [Test(Description = "После добавления двух записей, в репозитории должно быть две записи")]
        public async Task AddRangeAsync_Two_RepositoryHasOneEntity()
        {
            // Arrange
            var repository = _serviceProvider.GetRequiredService<IRepository<TestEntity>>();

            // Act
            await repository.AddRangeAsync([new TestEntity(), new TestEntity()], CancellationToken.None);

            // Assert
            var count = await repository.AsQueryable().CountAsync(CancellationToken.None);
            Assert.That(count, Is.EqualTo(2));
        }

        [Test(Description = "При фильтрации сущностей, не должны возвращаться отфильтрованные записи")]
        public async Task Where_FilterByKey_RepositoryHasOneEntity()
        {
            // Arrange
            var repository = _serviceProvider.GetRequiredService<IRepository<TestEntity>>();
            await repository.AddRangeAsync([new TestEntity() {  Key = "1" }, new TestEntity() { Key = "2" }], CancellationToken.None);

            // Act
            var result = await repository.Where(r => r.Key == "2").CountAsync(CancellationToken.None);

            // Assert
            Assert.That(result, Is.EqualTo(1));
        }

        [Test(Description = "После обновления, запись должна быть изменена")]
        public async Task UpdateAsync_UpdateKey_RepositoryHasOneEntity()
        {
            // Arrange
            var repository = _serviceProvider.GetRequiredService<IRepository<TestEntity>>();
            var entity = new TestEntity() { Key = "1" };
            await repository.AddAsync(entity, CancellationToken.None);
            entity.Key = "2";

            // Act
            await repository.UpdateAsync(entity, CancellationToken.None);

            // Assert
            var result = await repository.AsQueryable().ToArrayAsync(CancellationToken.None);
            Assert.That(result.Length, Is.EqualTo(1));
            Assert.That(result[0].Key, Is.EqualTo("2"));
        }

        [Test(Description = "При обновлении записи, которой нет в БД, должно быть исключение")]
        public void UpdateAsync_UpdateNotExistedEntity_ThrowDbUpdateConcurrencyException()
        {
            // Arrange
            var repository = _serviceProvider.GetRequiredService<IRepository<TestEntity>>();
            var entity = new TestEntity() { Key = "1" };
            
            // Act
            // Assert
            Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () =>
                await repository.UpdateAsync(entity!, CancellationToken.None));
        }
    }
}
