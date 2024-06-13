using Meta.Common.Hosts.Features.AppFeatures.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Meta.Common.Hosts.Grpc.Features.AppFeatures.ExceptionHandler
{
    /// <summary>
    /// Функциональность обработки исключений.
    /// </summary>
    internal class ExceptionHandlerFeature : AppFeature
    {
        /// <inheritdoc/>
        public override void AddFeature(IServiceCollection services)
        {
            services.AddGrpc(options =>
            {
                options.Interceptors.Add<ExceptionHandlingInterceptor>();
            });
        }
    }
}
