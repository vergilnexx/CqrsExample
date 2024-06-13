using Grpc.Core;
using Grpc.Net.Client;
using Meta.Common.Contracts.Options;
using System.Net;

namespace Meta.Common.Clients
{
    /// <summary>
    /// GRPC клиент.
    /// </summary>
    public abstract class BaseGrpcClient
    {
        /// <summary>
        /// Возвращает канал с данными авторизации.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <returns>Канал.</returns>
        public static GrpcChannel GetAuthorizationChannel(string url)
        {
            CallCredentials credentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                metadata.Add(HttpRequestHeader.Authorization.ToString(), $"{TrustedNetworkOptions.Scheme} {TrustedNetworkOptions.Service}");
                return Task.CompletedTask;
            });
            GrpcChannelOptions options = new()
            {
                Credentials = ChannelCredentials.Create(ChannelCredentials.Insecure, credentials),
                UnsafeUseInsecureChannelCallCredentials = true
            };
            var channel = GrpcChannel.ForAddress(url, options);
            return channel;
        }
    }
}
