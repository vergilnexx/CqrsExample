namespace Meta.Common.Hosts.RabbitMq.Registrar.Client
{
    /// <summary>
    /// Фабрика клиентов RabbitMq.
    /// </summary>
    public interface IRabbitMqClientFactory
    {
        /// <summary>
        /// Возвращает клиента.
        /// </summary>
        /// <returns>Клиент.</returns>
        IRabbitMqClient GetClient();
    }
}
