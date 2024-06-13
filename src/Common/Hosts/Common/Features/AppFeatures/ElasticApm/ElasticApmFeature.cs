using Meta.Common.Hosts.Features.AppFeatures.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Meta.Common.Hosts.Features.AppFeatures.ElasticApm
{
    /// <summary>
    /// Функциональность Elastic.Apm.
    /// </summary>
    internal class ElasticApmFeature : AppFeature
    {
        /// <inheritdoc/>
        public override void AddFeature(IServiceCollection services)
        {
            base.AddFeature(services);

            services.AddAllElasticApm();
        }
    }
}
