namespace Meta.Common.Hosts.RabbitMq.Options
{
    /// <summary>
    /// Настройки RabbitMq.
    /// </summary>
    public class RabbitMqOptions
    {
        /// <summary>
        /// Название секции в конфигурации для сервиса.
        /// </summary>
        public static readonly string ServiceSectionName = "RabbitMqService";

        /// <summary>
        /// Название секции в конфигурации для клиента.
        /// </summary>
        public static readonly string ClientSectionName = "RabbitMqClient";

        /// <summary>
        /// Логин.
        /// </summary>
        public required string UserName { get; set; }

        /// <summary>
        /// Пароль.
        /// </summary>
        public required string Password { get; set; }

        /// <summary>
        /// Список URI.
        /// </summary>
        public string[] Hosts { get; set; } = [];

        /// <summary>
        /// Наименование виртуального хоста.
        /// </summary>
        public required string VirtualHost { get; set; }

        /// <summary>
        /// Порт.
        /// </summary>
        public required string Port { get; set; }

        /// <summary>
        /// Интервал периодической отправки сигнала для проверки наличия соединения с сервером RabbitMQ.
        /// </summary>
        public TimeSpan Heartbeat
        {
            get
            {
                return _heartbeat;
            }
            set
            {
                if (value.TotalSeconds < 0.0 || value.TotalSeconds > ushort.MaxValue)
                {
                    throw new ArgumentOutOfRangeException($@"Значение параметра: {nameof(Heartbeat)} должно быть в интервале: 
                        [{TimeSpan.FromSeconds(0)}, {TimeSpan.FromSeconds(ushort.MaxValue)}].");
                }

                _heartbeat = value;
            }
        }

        private TimeSpan _heartbeat = TimeSpan.FromSeconds(60);

        /// <summary>
        /// Количество сообщений, которые будут обрабатываться параллельно.
        /// </summary>
        public int PrefetchCount { get; set; } = 10;

        /// <summary>
        /// Количество повторных попыток обработки сообщения.
        /// </summary>
        public int RetryCount { get; set; } = 5;

        /// <summary>
        /// Интервал времени повторной попытки обработки сообщения.
        /// </summary>
        public TimeSpan RetryInterval { get; set; } = TimeSpan.FromSeconds(15);
    }
}
