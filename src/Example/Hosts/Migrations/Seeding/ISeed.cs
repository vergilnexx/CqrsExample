using Meta.Example.Infrastructures.DataAccess.Contexts;

namespace Meta.Example.Migrations.Seeding
{
    /// <summary>
    /// Интерфейс seed'инга.
    /// </summary>
    internal interface ISeed
    {
        /// <summary>
        /// Заполнение данными.
        /// </summary>
        /// <param name="context">Контекст.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        Task FillAsync(ExampleDbContext context, CancellationToken cancellationToken);
    }
}
