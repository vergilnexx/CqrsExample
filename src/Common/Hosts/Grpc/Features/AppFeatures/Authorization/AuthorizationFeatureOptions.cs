using Microsoft.Extensions.Configuration;

namespace Meta.Common.Hosts.Grpc.Features.AppFeatures.Authorization
{
    /// <summary>
    /// Настройки  функциональности авторизации.
    /// </summary>
    internal class AuthorizationFeatureOptions
    {
        /// <summary>
        /// Секции с настройками.
        /// </summary>
        public IReadOnlyCollection<IConfigurationSection> Sections { get; set; } = [];
    }
}
