using Microsoft.EntityFrameworkCore;

namespace Meta.Example.Infrastructures.DataAccess.Contexts
{
    /// <summary>
    /// Контекст базы данных примера.
    /// </summary>
    /// <param name="options">Настройки.</param>
    public class ExampleDbContext(DbContextOptions<ExampleDbContext> options) : DbContext(options)
    {
        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            CustomModelBuilder.OnModelCreating(modelBuilder);
        }
    }
}
