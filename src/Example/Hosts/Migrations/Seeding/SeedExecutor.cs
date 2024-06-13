using Meta.Common.Contracts.Exceptions.Common;
using Meta.Example.Infrastructures.DataAccess.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Meta.Example.Migrations.Seeding
{
    /// <summary>
    /// Применение Seed'ов.
    /// </summary>
    /// <remarks>
    /// Конструктор.
    /// </remarks>
    /// <param name="_dbContext">Репозиторий ключей.</param>
    internal class SeedExecutor(DbContext _dbContext)
    {
        /// <summary>
        /// Применение Seed'ов.
        /// </summary>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <exception cref="NotFoundException">Исключение, если не удалось получить контекст базы данных.</exception>
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var context = _dbContext as ExampleDbContext 
                            ?? throw new NotFoundException("Не удалось получить контекст доступа к базе данных");

            var type = typeof(ISeed);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(a => a.GetTypes())
                        .Where(t => type.IsAssignableFrom(t) && !t.IsInterface);
            foreach (var seedType in types)
            {
                var seed = (ISeed?)Activator.CreateInstance(seedType);
                if (seed != null)
                {
                    await seed.FillAsync(context, cancellationToken);
                }
            }

            context.SaveChanges();
        }
    }
}
