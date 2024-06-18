using MassTransit;
using Meta.Common.Contracts.Events;
using Meta.Common.Hosts.RabbitMq.Options;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace Meta.Common.Hosts.RabbitMq.Registrar.Service
{
    /// <inheritdoc/>
    public sealed class RabbitMqServiceFactory : IRabbitMqServiceFactory
    {
        private const string Protocol = "rabbitmq://";
        private readonly RabbitMqOptions _options;
        private readonly IEnumerable<INamedConsumer> _consumers;

        private readonly ConcurrentDictionary<string, Lazy<IBusControl>> _busControls;

        /// <summary>
        /// Инициализирует экземпляр <see cref="RabbitMqServiceFactory"/>.
        /// </summary>
        public RabbitMqServiceFactory(IOptions<RabbitMqOptions> options, IEnumerable<INamedConsumer> consumers)
        {
            _options = options.Value;
            _consumers = consumers ?? [];
            _busControls = new ConcurrentDictionary<string, Lazy<IBusControl>>(
                StringComparer.InvariantCultureIgnoreCase);
        }

        /// <inheritdoc/>
        public IHostedService GetService()
        {
            var controlProvider = _busControls.GetOrAdd(string.Empty, _ =>
            {
                return new Lazy<IBusControl>(() =>
                {
                    return CreateBusControl();
                });
            });

            return new RabbitMqHostedService(controlProvider.Value);
        }

        private IBusControl CreateBusControl()
        {
            return Bus.Factory.CreateUsingRabbitMq(busCfg =>
            {
                var hosts = _options.ParsedHosts();
                var mainHost = hosts.FirstOrDefault();
                var uri = new Uri($"{Protocol}{mainHost}/{_options.VirtualHost}", UriKind.Absolute);
                busCfg.Host(uri, hostCfg =>
                {
                    hostCfg.Username(_options.UserName);
                    hostCfg.Password(_options.Password);

                    if (hosts.Length > 1)
                    {
                        hostCfg.UseCluster(c =>
                        {
                            foreach (var node in hosts)
                            {
                                c.Node(node);
                            }
                        });
                    }
                    hostCfg.Heartbeat((ushort)_options.Heartbeat.TotalSeconds);
                });

                foreach (var consumer in _consumers)
                {
                    var consumerType = consumer.GetType();
                    busCfg.ReceiveEndpoint(consumer.QueueName, e =>
                    {
                        e.PrefetchCount = _options.PrefetchCount;
                        e.UseMessageRetry(r => r.Interval(_options.RetryCount, _options.RetryInterval));
                        
                        e.Consumer(consumerType, _ =>
                        {
                            return _consumers.FirstOrDefault(c => c.GetType() == consumerType);
                        });
                    });
                }
            });
        }
    }
}
