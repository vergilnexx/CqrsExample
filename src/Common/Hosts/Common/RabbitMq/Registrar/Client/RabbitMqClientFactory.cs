using MassTransit;
using Meta.Common.Hosts.RabbitMq.Options;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace Meta.Common.Hosts.RabbitMq.Registrar.Client
{
    /// <inheritdoc/>
    public sealed class RabbitMqClientFactory : IRabbitMqClientFactory
    {
        private const string Protocol = "rabbitmq://";
        private readonly RabbitMqOptions _options;

        private readonly ConcurrentDictionary<string, Lazy<IBusControl>> _busControls;

        /// <summary>
        /// Инициализирует экземпляр <see cref="RabbitMqClientFactory"/>.
        /// </summary>
        public RabbitMqClientFactory(IOptions<RabbitMqOptions> options)
        {
            _options = options.Value;
            _busControls = new ConcurrentDictionary<string, Lazy<IBusControl>>(
                StringComparer.InvariantCultureIgnoreCase);
        }

        /// <inheritdoc/>
        public IRabbitMqClient GetClient()
        {
            var controlProvider = _busControls.GetOrAdd(string.Empty, _ =>
            {
                return new Lazy<IBusControl>(() =>
                {
                    return CreateBusControl();
                });
            });

            return new RabbitMqClient(controlProvider.Value);
        }

        private IBusControl CreateBusControl()
        {
            return Bus.Factory.CreateUsingRabbitMq(busCfg =>
            {
                var mainHost = _options.Hosts.FirstOrDefault();
                var uri = new Uri($"{Protocol}{mainHost}/{_options.VirtualHost}", UriKind.Absolute);
                busCfg.Host(uri, hostCfg =>
                {
                    hostCfg.Username(_options.UserName);
                    hostCfg.Password(_options.Password);

                    if (_options.Hosts.Length > 1)
                    {
                        hostCfg.UseCluster(c =>
                        {
                            foreach (var node in _options.Hosts)
                            {
                                c.Node(node);
                            }
                        });
                    }
                    hostCfg.Heartbeat((ushort)_options.Heartbeat.TotalSeconds);
                });
            });
        }
    }
}
