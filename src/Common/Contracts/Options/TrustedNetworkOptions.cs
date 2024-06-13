using Microsoft.AspNetCore.Authentication;

namespace Meta.Common.Contracts.Options
{
    /// <summary>
    /// Настройки аутентификации по доверенным сетям.
    /// </summary>
    public class TrustedNetworkOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        /// Название схемы.
        /// </summary>
        public static readonly string Scheme = "TrustedNetwork";

        /// <summary>
        /// Служба.
        /// </summary>
        public static readonly string Service = "Service";

        /// <summary>
        /// Список доверенных сетей.
        /// </summary>
        public string TrustedNetworks { get; set; } = string.Empty;
    }
}
