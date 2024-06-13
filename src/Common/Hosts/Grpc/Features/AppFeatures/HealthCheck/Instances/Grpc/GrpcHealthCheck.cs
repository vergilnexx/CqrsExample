using Grpc.Health.V1;
using Grpc.HealthCheck;
using Grpc.Net.Client;
using Meta.Common.Hosts.Features.AppFeatures.HealthCheck.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Globalization;
using Health = Grpc.Health.V1.Health;

namespace Meta.Common.Hosts.Grpc.Features.AppFeatures.HealthCheck.Instances.Grpc
{
    /// <summary>
    /// Проверка на работоспособность API.
    /// </summary>
    internal class GrpcHealthCheck : IHealthCheck
    {
        private readonly IConfiguration _configuration;
        private readonly GrpcHealthCheckOptions _options;
        private readonly HealthServiceImpl _healthService;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="configuration">Конфигурация.</param>
        /// <param name="options">Настройки.</param>
        /// <param name="healthService">Сервис проверки.</param>
        public GrpcHealthCheck(IConfiguration configuration, 
            GrpcHealthCheckOptions options, 
            HealthServiceImpl healthService)
        {
            _configuration = configuration;
            _options = options;
            _healthService = healthService;
        }


        /// <summary>
        /// Проверка на работоспособность.
        /// </summary>
        /// <param name="context">Контекст.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Результат проверки.</returns>
        /// <exception cref="HealthCheckConfigurationException">Исключение, бросаемое когда операция не может быть выполнена.</exception>
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            if (!TimeSpan.TryParse(_options.Timeout, CultureInfo.CurrentCulture, out var timeout))
            {
                throw new HealthCheckConfigurationException($"Не удалось получить Timeout из {_options.Timeout}");
            }

            if (string.IsNullOrWhiteSpace(_options.UrlConfigSection))
            {
                throw new HealthCheckConfigurationException("Название секции с URL для проверки GRPC не может быть пустой.");
            }

            var url = _configuration.GetValue<string>(_options.UrlConfigSection);
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new HealthCheckConfigurationException($"Не удалось получить URL из секции '{_options.UrlConfigSection}' для проверки GRPC.");
            }
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                throw new HealthCheckConfigurationException($"URL({url}) из секции '{_options.UrlConfigSection}' имеет невалидный формат для проверки GRPC.");
            }

            _healthService.SetStatus(_options.Name, HealthCheckResponse.Types.ServingStatus.Serving);

            using var channel = GrpcChannel.ForAddress(url);
            var client = new Health.HealthClient(channel);

            var response = await client.CheckAsync(new HealthCheckRequest() { Service = _options.Name }, cancellationToken: cancellationToken);
            var status = response.Status;

            if (status == HealthCheckResponse.Types.ServingStatus.Serving)
            {
                return HealthCheckResult.Healthy();
            }

            return HealthCheckResult.Unhealthy(status.ToString());
        }
    }
}
