namespace Meta.Common.Hosts.RabbitMq.Registrar.Client
{
    /// <summary>
    /// Клиент RabbitMq.
    /// </summary>
    public interface IRabbitMqClient
    {
        /// <summary>
        /// Выполняет публикацию сообщения в шину.
        /// </summary>
        /// <param name="message">Сообщение.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <typeparam name="TMessage">Тип сообщения.</typeparam>
        Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken)
            where TMessage : class;
    }
}
