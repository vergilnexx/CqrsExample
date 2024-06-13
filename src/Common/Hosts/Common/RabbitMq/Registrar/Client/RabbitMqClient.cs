using MassTransit;
using Meta.Common.Contracts.Abstract;
using Meta.Common.Contracts.Events;
using Meta.Common.Hosts.RabbitMq.Exceptions;

namespace Meta.Common.Hosts.RabbitMq.Registrar.Client
{
    /// <summary>
    /// Клиент RabbitMq.
    /// </summary>
    /// <param name="_busControl">Управление шиной.</param>
    public class RabbitMqClient(IBusControl _busControl) : IRabbitMqClient
    {
        /// <inheritdoc/>
        public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken) where TMessage : class
        {
            ArgumentNullException.ThrowIfNull(message);

            try
            {
                var interfaces = message.GetType().GetInterfaces().Where(i => i != typeof(IEvent));
                foreach (var @interface in interfaces)
                {
                    await _busControl.Publish(message, @interface ?? message.GetType(), cancellationToken);
                }
            }
            catch (Exception e)
            {
                throw new RabbitMqServiceException($"Возникло исключение при публикации сообщения типа '{typeof(TMessage)}'", e);
            }
        }
    }
}
