using MassTransit;
using Meta.Common.Contracts.Abstract;

namespace Meta.Common.Contracts.Events
{
    /// <summary>
    /// Именованный обработчик.
    /// </summary>
    /// <typeparam name="TMessage">Тип сообщения.</typeparam>
    public interface INamedConsumer<in TMessage> : INamedConsumer, IConsumer<TMessage> where TMessage : class, IEvent
    {
    }

    /// <summary>
    /// Именованный обработчик.
    /// </summary>
    public interface INamedConsumer
    {
        /// <summary>
        /// Наименование очереди.
        /// </summary>
        public string QueueName { get; }
    }
}
