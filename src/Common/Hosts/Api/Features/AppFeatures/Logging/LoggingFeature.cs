using Meta.Common.Hosts.Features.AppFeatures.Base;
using Meta.Common.Hosts.Features.AppFeatures.Logging.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Settings.Configuration;

namespace Meta.Common.Hosts.Api.Features.AppFeatures.Logging
{
    /// <summary>
    /// Функциональность логирования.
    /// </summary>
    internal class LoggingFeature : AppFeature
    {
        /// <inheritdoc />
        public override void AddFeature(IServiceCollection services, IHostBuilder hostBuilder, ILoggingBuilder loggingBuilder)
        {
            base.AddFeature(services, hostBuilder, loggingBuilder);

            hostBuilder.UseSerilog(Configure);
        }

        internal static void Configure(HostBuilderContext host, LoggerConfiguration config)
        {
            var options = new ConfigurationReaderOptions { SectionName = "Features:Logging:Serilog" };
            config.ReadFrom.Configuration(host.Configuration, options)
                .Enrich.WithEnvironment(host.Configuration, host.HostingEnvironment);
        }
    }
}
