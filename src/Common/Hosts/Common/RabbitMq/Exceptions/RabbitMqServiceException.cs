namespace Meta.Common.Hosts.RabbitMq.Exceptions
{
    /// <summary>
    /// Исключение, бросаемое сервисом RabbitMq.
    /// </summary>
    public class RabbitMqServiceException : Exception
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="message">Сообщение об ошибке.</param>
        public RabbitMqServiceException(string message) : base(message)
        {
        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="message">Сообщение об ошибке.</param>
        /// <param name="innerException">Внутреннее исключение.</param>
        public RabbitMqServiceException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
