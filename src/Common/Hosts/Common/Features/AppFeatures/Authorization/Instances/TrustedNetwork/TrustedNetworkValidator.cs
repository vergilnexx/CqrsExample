using Meta.Common.Contracts.Options;
using Meta.Common.Hosts.Features.AppFeatures.Authorization.Instances.TrustedNetwork.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Net;

namespace Meta.Common.Hosts.Features.AppFeatures.Authorization.Instances.TrustedNetwork
{
    ///<inheritdoc/>
    /// <summary>
    /// Конструктор.
    /// </summary>
    public class TrustedNetworkValidator : ITrustedNetworkValidator
    {
        private static readonly string XRealIp = "X-Real-IP";
        private static readonly string XOriginalForwardedIp = "X-Original-Forwarded-For";

        private readonly TrustedNetworkOptions _options;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="options">Настройки.</param>
        public TrustedNetworkValidator(IOptions<TrustedNetworkOptions> options)
        {
            _options = options.Value;
        }

        /// <inheritdoc/>
        public TrustedNetworkValidationResult Validate(HttpRequest request)
        {
            List<string> list = [];
            IPAddress? originalIp = GetOriginalIp(request);

            IReadOnlyCollection<IPNetwork> source = ParseTrustedNetworks(_options.TrustedNetworks);

            var isTrustedNetwork = source.Any((ip) => IsInTrustedNetworks(originalIp, ip));
            if (!isTrustedNetwork)
            {
                list.Add($"Попытка аутентификации отклонена." +
                    $"Входящий IP: {originalIp} не входит в перечень доверенных сетей. " +
                    $"Доверенные IP: {_options.TrustedNetworks}.");
                list.AddRange(request.Headers.Select(header => $"Заголовки запроса: {header.Key}, {header.Value}"));
                return TrustedNetworkValidationResult.Error(originalIp, list);
            }

            return TrustedNetworkValidationResult.Success(originalIp);
        }

        private static IPAddress? GetOriginalIp(HttpRequest request)
        {
            return TryGetIpFromHeader(request, XOriginalForwardedIp)
                ?? TryGetIpFromHeader(request, XRealIp)
                ?? request.HttpContext.Connection.RemoteIpAddress;
        }

        private static IPAddress? TryGetIpFromHeader(HttpRequest request, string headerKey)
        {
            if (request.Headers.TryGetValue(headerKey, out StringValues value)
                && IPAddress.TryParse(value, out IPAddress? address))
            {
                return address;
            }

            return null;
        }

        private static bool IsInTrustedNetworks(IPAddress? address, IPNetwork network)
        {
            if (address == null)
            {
                return false;
            }

            IPAddress ipaddress = address.IsIPv4MappedToIPv6
                                    ? address.MapToIPv4()
                                    : address;
            return network.Contains(ipaddress);
        }

        private static IPNetwork[] ParseTrustedNetworks(string networks)
        {
            if (string.IsNullOrWhiteSpace(networks))
            {
                return [];
            }

            return networks
                    .Split(";", StringSplitOptions.RemoveEmptyEntries)
                    .Select(n => n.Trim())
                    .Where(n => !string.IsNullOrWhiteSpace(n))
                    .Select(IPNetwork.Parse)
                    .ToArray();
        }
    }
}
