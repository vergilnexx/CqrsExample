using Meta.Common.Contracts.Exceptions.Common;
using Meta.Example.Infrastructures.DataAccess.Contexts;
using Meta.Example.Migrations.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Meta.Example.Migrations
{
    /// <summary>
    /// Запуск миграций.
    /// </summary>
    internal sealed class Startup(
        IServiceProvider _serviceProvider,
        ILoggerFactory _loggerFactory)
    {

        /// <summary>
        /// Выполняет миграции включая применение сидинга.
        /// </summary>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <exception cref="NotFoundException">Исключение, если не удалось получить контекст базы данных.</exception>
        public async Task StartMigrationsAsync(CancellationToken cancellationToken)
        {
            var runnerLogger = _loggerFactory.CreateLogger<Startup>();

            await ExecuteMigrationsAsync(runnerLogger, cancellationToken);
            await ExecuteSeedingAsync(runnerLogger, cancellationToken);
        }

        /// <summary>
        /// Выполняет миграции из приложения.
        /// </summary>
        /// <param name="runnerLogger">Логгер.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <exception cref="NotFoundException">Исключение, если не удалось получить контекст базы данных.</exception>
        private async Task ExecuteMigrationsAsync(ILogger runnerLogger, CancellationToken cancellationToken)
        {
            runnerLogger.LogInformation($"{nameof(ExecuteMigrationsAsync)}: Запуск миграции");

            var dbContext = _serviceProvider.CreateScope().ServiceProvider.GetService<ExampleDbContext>()
                            ?? throw new NotFoundException("Не удалось получить контекст доступа к базе данных");
            
            try
            {
                runnerLogger.LogInformation($"{nameof(ExecuteMigrationsAsync)}: Попытка подключиться к базе данных");
                await dbContext.Database.MigrateAsync(cancellationToken);
            }
            catch (Exception e)
            {
                runnerLogger.LogError(e, $"{nameof(ExecuteMigrationsAsync)}: Не удалось подключиться к базе данных");
                throw;
            }

            runnerLogger.LogInformation($"{nameof(ExecuteMigrationsAsync)}: Все миграции накачены");
        }

        /// <summary>
        /// Выполняет сидинг.
        /// </summary>
        /// <param name="runnerLogger">Логгер.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <exception cref="NotFoundException">Исключение, если не удалось получить контекст базы данных.</exception>
        private async Task ExecuteSeedingAsync(ILogger runnerLogger, CancellationToken cancellationToken)
        {
            runnerLogger.LogInformation($"{nameof(ExecuteSeedingAsync)}: Запуск наполнения базы данных");

            var dbContext = _serviceProvider.CreateScope().ServiceProvider.GetService<ExampleDbContext>()
                            ?? throw new NotFoundException("Не удалось получить контекст доступа к базе данных");
            var seedExecutor = new SeedExecutor(dbContext);

            try
            {
                runnerLogger.LogInformation($"{nameof(ExecuteSeedingAsync)}: Попытка подключиться к базе данных");
                await seedExecutor.RunAsync(cancellationToken);
            }
            catch (Exception e)
            {
                runnerLogger.LogError(e, $"{nameof(ExecuteSeedingAsync)}: Не удалось подключиться к базе данных");
                throw;
            }

            runnerLogger.LogInformation($"{nameof(ExecuteSeedingAsync)}: Наполнение базы завершено");
        }
    }
}
