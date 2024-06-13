using Meta.Common.Infrastructures.DataAccess.Configurators;
using Meta.Example.Infrastructures.DataAccess.Contexts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Meta.Example.Infrastructures.DataAccess.Common.Configurators
{
    /// <summary>
    /// Конфигуратор базы данных примера.
    /// </summary>
    /// <param name="configuration">Конфигурация.</param>
    /// <param name="loggerFactory">Фабрика логгера.</param>
    public class ExampleDbContextConfigurator(IConfiguration configuration, ILoggerFactory loggerFactory)
        : BaseDbContextConfigurator<ExampleDbContext>(configuration, loggerFactory)
    {
        /// <inheritdoc/>
        protected override string ConnectionStringName => "ExampleDb";
    }
}
