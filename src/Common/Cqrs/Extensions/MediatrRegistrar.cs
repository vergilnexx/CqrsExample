using MediatR;
using Meta.Common.Applications.AppServices.Contexts.Common.Services.DiagnosticProvider;
using Meta.Common.Cqrs.Behaviors;
using Meta.Common.Cqrs.Behaviors.Diagnostic;
using Meta.Common.Cqrs.Behaviors.Events;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Meta.Common.Cqrs.Extensions
{
    /// <summary>
    /// Регистрация медиатра и важнейших поведений.
    /// </summary>
    public static class MediatrRegistrar
    {
        /// <summary>
        /// Регистрирует медиатр.
        /// </summary>
        /// <param name="services">Коллекция сервисов.</param>
        /// <param name="assembly">Информация о сборке.</param>
        /// <returns>Коллекция сервисов.</returns>
        public static IServiceCollection AddMediatR(this IServiceCollection services, Assembly assembly)
        {
            services
                .AddMediatR(cfg =>
                {
                    cfg.RegisterServicesFromAssembly(assembly);
                })
                .AddScoped<IEventMessageProvider, EventMessageProvider>()
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationRequestBehavior<,>))
                .AddScoped<IDiagnosticProvider, DiagnosticProvider>()
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(DiagnosticBehavior<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(EventSendingBehavior<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionalBehavior<,>));

            return services;
        }
    }
}
