using Microsoft.Extensions.Hosting;

namespace Meta.Common.Hosts.RabbitMq.Registrar.Service
{
    /// <summary>
    /// Фабрика сервисов RabbitMq.
    /// </summary>
    public interface IRabbitMqServiceFactory
    {
        /// <summary>
        /// Возвращает сервис.
        /// </summary>
        /// <returns>Хост.</returns>
        IHostedService GetService();
    }
}
