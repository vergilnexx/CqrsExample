using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Meta.Common.Hosts.Api.Features.AppFeatures.HealthCheck.Instances.RabbitMq
{
    /// <summary>
    /// Настройки проверок работоспособности RabbitMq.
    /// </summary>
    internal class RabbitMqHealthCheckOptions
    {
        /// <summary>
        /// Наименование.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Конфигурационная секция URL к API.
        /// </summary>
        public required string ConfigSection { get; set; }

        /// <summary>
        /// Тайм-аут подключения
        /// </summary>
        public string Timeout { get; set; } = TimeSpan.FromSeconds(3).ToString();

        /// <summary>
        /// Какой статус отдать при неудачной проверке
        /// <see cref="HealthStatus"/>
        /// </summary>
        public string FailureStatus { get; set; } = HealthStatus.Degraded.ToString();

        /// <summary>
        /// Тэги.
        /// </summary>
        public string[] Tags { get; set; } = [];

    }
}
