using MassTransit;
using Meta.Common.Hosts.RabbitMq.Exceptions;
using Microsoft.Extensions.Hosting;

namespace Meta.Common.Hosts.RabbitMq.Registrar.Service
{
    /// <summary>
    /// Сервис RabbitMq.
    /// </summary>
    /// <param name="_busControl">Управление шиной.</param>
    public class RabbitMqHostedService(IBusControl _busControl) : IHostedService
    {
        /// <inheritdoc/>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _busControl.StartAsync(cancellationToken);
            }
            catch (Exception e)
            {
                var wrappedException = new RabbitMqServiceException($"Возникло исключение при запуске демона RabbitMQ", e);
                throw wrappedException;
            }
        }

        /// <inheritdoc/>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _busControl.StopAsync(cancellationToken);
            }
            catch (Exception e)
            {
                var wrappedException = new RabbitMqServiceException($"Возникло исключение при завершении работы демона RabbitMQ", e);
                throw wrappedException;
            }
        }
    }
}
