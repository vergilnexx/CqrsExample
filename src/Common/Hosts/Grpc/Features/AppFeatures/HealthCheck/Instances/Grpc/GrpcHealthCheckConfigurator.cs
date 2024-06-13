using Grpc.AspNetCore.HealthChecks;
using Grpc.HealthCheck;
using Meta.Common.Hosts.Features.AppFeatures.HealthCheck.Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Meta.Common.Hosts.Grpc.Features.AppFeatures.HealthCheck.Instances.Grpc
{
    /// <summary>
    /// Проверка на работоспособность для GRPC.
    /// </summary>
    internal class GrpcHealthCheckConfigurator : IHealthCheckConfigurator, IHealthCheckMapConfigurator
    {
        private const string OptionsSectionName = "Options";

        /// <inheritdoc/>
        public void MapHealthCheckService(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGrpcService<HealthServiceImpl>();
        }

        /// <inheritdoc/>
        public void Configure(IServiceCollection services, IConfiguration configuration, IConfigurationSection optionsSection, IHealthChecksBuilder checksBuilder)
        {
            var options = optionsSection.GetSection(OptionsSectionName).Get<GrpcHealthCheckOptions>();
            if (options == null)
            {
                return;
            }
            
            services.AddSingleton(options);
            if (!Enum.TryParse<HealthStatus>(options.FailureStatus, out var failureStatus))
            {
                throw new HealthCheckConfigurationException($"Не удалось получить статус из {options.FailureStatus}");
            }

            services.TryAddSingleton<HealthServiceImpl>();
            services.Configure<GrpcHealthChecksOptions>(o =>
            {
                o.Services.Map(options.Name, r => r.Tags.Any(t => options.Tags.Contains(t)));
            });
            checksBuilder.AddCheck<GrpcHealthCheck>(options.Name, failureStatus, options.Tags);
        }
    }
}
