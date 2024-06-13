using Meta.Common.Hosts.Features.AppFeatures.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Meta.Common.Hosts.Consumer.Extensions
{
    /// <summary>
    /// Регистрация для хостов.
    /// </summary>
    public static class HostRegistrar
    {
        /// <summary>
        /// Регистрация функциональностей.
        /// </summary>
        /// <param name="services">Коллекция сервисов.</param>
        /// <param name="hostBuilder">Builder хоста.</param>
        /// <param name="loggingBuilder">Builder логирования.</param>
        /// <param name="configuration">Конфигурация.</param>
        /// <param name="featureAssemblies">Сборки, где находятся функциональности.</param>
        /// <returns>Коллекция сервисов.</returns>
        public static IServiceCollection AddFeatures(this IServiceCollection services, 
            IHostApplicationBuilder hostBuilder, ILoggingBuilder loggingBuilder, IConfiguration configuration, Assembly[] featureAssemblies)
        {
            services.AddHttpClient();

            IReadOnlyCollection<IAppFeature> features = AppFeatureFactory.GetAppFeatures(configuration, featureAssemblies);
            foreach (var feature in features.OrderBy(f => f.Order))
            {
                feature.AddFeature(services, hostBuilder, loggingBuilder);
                services.AddSingleton(feature);
            }
            return services;
        }
    }
}
