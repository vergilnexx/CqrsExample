using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Meta.Common.Hosts.Api.Features.AppFeatures.Authorization.Instances.Keycloak.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Meta.Common.Hosts.Api.Features.AppFeatures.Authorization.Instances.Keycloak
{
    /// <summary>
    /// Расширение для авторизации через Keycloak.
    /// </summary>
    internal static class KeycloakAuthorizationFeatureExtension
    {
        /// <summary>
        /// Регистрирует компоненты для аутентификации через Keycloak.
        /// </summary>
        /// <param name="services">Коллекция сервисов.</param>
        /// <param name="optionSection">Конфигурация.</param>
        /// <returns>Коллекция сервисов.</returns>
        public static IServiceCollection AddKeycloakAuthenticate(this IServiceCollection services, IConfigurationSection optionSection)
        {
            var keycloakAuthorizationOptions = optionSection.GetSection(KeycloakAuthorizationOptions.Scheme).Get<KeycloakAuthorizationOptions>();
            services.AddHttpClient();
            services.AddOptions<KeycloakAuthorizationOptions>()
                    .Configure(o =>
                    {
                        optionSection.Bind(KeycloakAuthorizationOptions.Scheme, o);
                    });

            services.AddAuthentication("Bearer")
                    .AddJwtBearer(options =>
                    {
                        GetJwtBearerSettings(options, keycloakAuthorizationOptions);
                    });

            return services;
        }

        /// <summary>
        /// Возвращает настройки для авторизации по токену.
        /// </summary>
        /// <param name="options">настройки токена.</param>
        /// <param name="keycloakAuthorizationOptions">настройки keycloak</param>
        internal static void GetJwtBearerSettings(JwtBearerOptions options, KeycloakAuthorizationOptions? keycloakAuthorizationOptions)
        {
            if (keycloakAuthorizationOptions == null)
            {
                return;
            }

            options.Authority = keycloakAuthorizationOptions.Authority;
            options.Audience = keycloakAuthorizationOptions.ClientId;
            options.RequireHttpsMetadata = keycloakAuthorizationOptions.RequireHttpsMetadata;
            options.MetadataAddress = keycloakAuthorizationOptions.MetadataAddress;

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    var message = "При авторизация произошла ошибка: " + context.Exception.Message;
                    var errorResponse = JsonSerializer.Serialize(new { message });
                    return context.Response.WriteAsync(errorResponse);
                }
            };
        }
    }
}
