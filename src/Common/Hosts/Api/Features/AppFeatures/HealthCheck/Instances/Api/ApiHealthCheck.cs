﻿using Meta.Common.Hosts.Features.AppFeatures.HealthCheck.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Globalization;

namespace Meta.Common.Hosts.Api.Features.AppFeatures.HealthCheck.Instances.Api
{
    /// <summary>
    /// Проверка на работоспособность API.
    /// </summary>
    /// <param name="_configuration">Конфигурация.</param>
    /// <param name="_httpClientFactory">Фабрика клиентов HTTP.</param>
    /// <param name="_options">Настройки.</param>
    internal class ApiHealthCheck(IConfiguration _configuration, IHttpClientFactory _httpClientFactory, ApiHealthCheckOptions _options) : IHealthCheck
    {
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
                throw new HealthCheckConfigurationException("Название секции с URL для проверки API не может быть пустой.");
            }

            var url = _configuration.GetValue<string>(_options.UrlConfigSection);
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new HealthCheckConfigurationException($"Не удалось получить URL из секции '{_options.UrlConfigSection}' для проверки API.");
            }
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                throw new HealthCheckConfigurationException($"URL({url}) из секции '{_options.UrlConfigSection}' имеет невалидный формат для проверки API.");
            }

            using var httpClient = _httpClientFactory.CreateClient();
            httpClient.Timeout = timeout;
            var response = await httpClient.GetAsync(url, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Healthy();
            }

            return HealthCheckResult.Unhealthy(response.StatusCode.ToString());
        }
    }
}