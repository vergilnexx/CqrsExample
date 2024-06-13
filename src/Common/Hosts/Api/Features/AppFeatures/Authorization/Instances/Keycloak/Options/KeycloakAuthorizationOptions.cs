using Microsoft.AspNetCore.Authentication;

namespace Meta.Common.Hosts.Api.Features.AppFeatures.Authorization.Instances.Keycloak.Options
{
    /// <summary>
    /// Настройки авторизации через Keycloak.
    /// </summary>
    internal class KeycloakAuthorizationOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        /// Название схемы.
        /// </summary>
        public static readonly string Scheme = "Keycloak";

        /// <summary>
        /// URL аутентификации.
        /// </summary>
        public string LoginUrl { get; set; } = string.Empty;

        /// <summary>
        /// URL.
        /// </summary>
        public string Authority { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор клиента.
        /// </summary>
        public string ClientId { get; set; } = string.Empty;

        /// <summary>
        /// Признак, что требуется https.
        /// </summary>
        public bool RequireHttpsMetadata { get; set; }

        /// <summary>
        /// Адрес с метаданными.
        /// </summary>
        public string MetadataAddress { get; set; } = string.Empty;
    }
}
