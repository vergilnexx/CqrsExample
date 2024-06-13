using Google.Protobuf.WellKnownTypes;
using Meta.Common.Hosts.Features.AppFeatures.Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Prometheus;

namespace Meta.Common.Hosts.Api.Features.AppFeatures.Prometheus
{
    /// <summary>
    /// Функциональность Prometheus.
    /// </summary>
    internal class PrometheusFeature : AppFeature
    {
        /// <inheritdoc/>
        public override void UseFeature(IApplicationBuilder application, IWebHostEnvironment environment)
        {
            base.UseFeature(application, environment);

            application.UseMetricServer();
            application.UseHttpMetrics(options =>
            {
                options.AddCustomLabel("host", context => context.Request.Host.Host);
            });
        }
    }
}
