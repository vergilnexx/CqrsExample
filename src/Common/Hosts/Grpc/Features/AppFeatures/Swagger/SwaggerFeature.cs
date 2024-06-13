using Meta.Common.Hosts.Features.AppFeatures.Base;
using Meta.Common.Hosts.Features.AppFeatures.Swagger;
using Meta.Common.Hosts.Features.AppFeatures.Swagger.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Meta.Common.Hosts.Grpc.Features.AppFeatures.Swagger
{
    /// <summary>
    /// функциональность swagger.
    /// </summary>
    internal class SwaggerFeature : AppFeature
    {
        public override void AddFeature(IServiceCollection services, IHostBuilder hostBuilder, ILoggingBuilder loggingBuilder)
        {
            base.AddFeature(services, hostBuilder, loggingBuilder);

            AddSwagger(services);
        }

        /// <inheritdoc />
        public override void UseFeature(IApplicationBuilder application, IWebHostEnvironment environment)
        {
            base.UseFeature(application, environment);

            application.UseSwagger();
            application.UseSwaggerUI();
        }

        /// <summary>
        /// Регистрация swagger.
        /// </summary>
        /// <param name="services">Коллекция сервисов.</param>
        /// <returns>Коллекция сервисов.</returns>
        private void AddSwagger(IServiceCollection services)
        {
            services.AddGrpcSwagger();
            services.AddSwaggerGen(Configure);
        }

        internal void Configure(SwaggerGenOptions options)
        {
            options.OperationFilter<AddTrustedNetworkHeaderFilter>();
            SwaggerConfigure.MapTypes(options);

            RegisterXmlComments(options);
        }
        
        private static void RegisterXmlComments(SwaggerGenOptions options)
        {
            var assembly = Assembly.GetExecutingAssembly();
            IncludeXmlComments(options, assembly);

            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
            {
                IncludeXmlComments(options, entryAssembly);
            }
        }

        private static void IncludeXmlComments(SwaggerGenOptions options, Assembly assembly)
        {
            var xmlFile = $"{assembly.GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (Path.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
                options.IncludeGrpcXmlComments(xmlPath, includeControllerXmlComments: true);
            }
        }
    }
}
