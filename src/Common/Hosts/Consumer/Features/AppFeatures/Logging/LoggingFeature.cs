using Meta.Common.Hosts.Features.AppFeatures.Base;
using Meta.Common.Hosts.Features.AppFeatures.Logging.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Settings.Configuration;

namespace Meta.Common.Hosts.Consumer.Features.AppFeatures.Logging
{
    /// <summary>
    /// Функциональность логирования.
    /// </summary>
    internal class LoggingFeature : AppFeature
    {
        /// <inheritdoc />
        public override void AddFeature(IServiceCollection services, IHostApplicationBuilder hostBuilder, ILoggingBuilder loggingBuilder)
        {
            base.AddFeature(services, hostBuilder, loggingBuilder);

            var options = new ConfigurationReaderOptions { SectionName = "Features:Logging:Serilog" };
            var configuration = new LoggerConfiguration();

            services.AddLogging(builder =>
                builder.AddSerilog(configuration
                                        .ReadFrom.Configuration(Configuration!, options)
                                        .Enrich.WithEnvironment(Configuration!, environment: null)
                       .CreateLogger()));
        }
    }
}
