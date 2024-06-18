using Meta.Common.Hosts.Api.Features.AppFeatures.HealthCheck.Instances.RabbitMq;
using Meta.Common.Hosts.Features.AppFeatures.HealthCheck.Base;
using Meta.Common.Hosts.RabbitMq.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;
using System.Globalization;

namespace Meta.Common.Hosts.Features.AppFeatures.HealthCheck.Instances.RabbitMq
{
    /// <summary>
    /// Конфигуратор проверок работоспособности RabbitMq.
    /// </summary>
    internal class RabbitMqHealthCheckConfigurator : IHealthCheckConfigurator
    {
        private const string OptionsSectionName = "Options";
        private const string connectionStringFormat = "amqp://{0}:{1}@{2}:{3}/{4}";

        /// <inheritdoc/>
        public void Configure(IServiceCollection services, IConfiguration configuration, IConfigurationSection optionsSection, IHealthChecksBuilder checksBuilder)
        {
            var options = optionsSection.GetSection(OptionsSectionName).Get<RabbitMqHealthCheckOptions>();
            if (options == null)
            {
                return;
            }

            services.AddSingleton(options);
            if (!Enum.TryParse<HealthStatus>(options.FailureStatus, out var failureStatus))
            {
                throw new HealthCheckConfigurationException($"Не удалось получить статус из {options.FailureStatus}");
            }

            if (!TimeSpan.TryParse(options.Timeout, CultureInfo.CurrentCulture, out var timeout))
            {
                throw new HealthCheckConfigurationException($"Не удалось получить Timeout из {options.Timeout}");
            }

            var rabbitMqOptions = configuration.GetSection(options.ConfigSection).Get<RabbitMqOptions>()
                                    ?? throw new HealthCheckConfigurationException($"Не удалось получить connectionString из секции '{options.ConfigSection}' для проверки RabbitMq.");

            var hosts = rabbitMqOptions.ParsedHosts();
            if (hosts.Length == 0)
            {
                throw new HealthCheckConfigurationException($"Не задан хост в секции '{options.ConfigSection}' для проверки RabbitMq.");
            }

            if (string.IsNullOrWhiteSpace(rabbitMqOptions.VirtualHost))
            {
                throw new HealthCheckConfigurationException($"Не задан виртуальный хост в секции '{options.ConfigSection}' для проверки RabbitMq.");
            }

            var addedHost = new HashSet<string>();
            foreach (var host in hosts)
            {
                if (addedHost.Contains(host))
                {
                    continue;
                }

                ConfigureHost(services, checksBuilder, options, host, failureStatus, timeout, rabbitMqOptions);
                addedHost.Add(host);
            }
        }

        private static void ConfigureHost(IServiceCollection services, IHealthChecksBuilder checksBuilder, RabbitMqHealthCheckOptions options, string host,
            HealthStatus failureStatus, TimeSpan timeout, RabbitMqOptions rabbitMqOptions)
        {
            try
            {
                var url = string.Format(connectionStringFormat,
                    rabbitMqOptions.UserName, rabbitMqOptions.Password, host, rabbitMqOptions.Port, rabbitMqOptions.VirtualHost);
                var factory = new ConnectionFactory()
                {
                    Uri = new Uri(url),
                    AutomaticRecoveryEnabled = true
                };

                var connection = factory.CreateConnection();
                services.AddSingleton(connection);
                checksBuilder.AddRabbitMQ(options.Name, failureStatus, options.Tags, timeout);
            }
            catch (Exception e)
            {
                throw new HealthCheckConfigurationException($"Произошла ошибка при инициализации проверки RabbitMq.", e);
            }
        }
    }
}
