using Grpc.Core;
using MediatR;
using Meta.Common.Hosts.Attributes.TrustedNetwork;
using Meta.Common.Hosts.Grpc.Services;
using Meta.Example.Applications.Handlers.Contexts.Forecast.Commands.DeleteWeatherForecast;
using Meta.Example.Private.Hosts.Grpc.Protos;
using static Meta.Example.Private.Hosts.Grpc.Protos.WeatherForecastService;

namespace Meta.Example.Private.Hosts.Grpc.Services
{
    /// <summary>
    /// Сервис предсказаний.
    /// </summary>
    /// <param name="_mediator">Обработчик сообщений.</param>
    public class WeatherForecastPresentationService(IMediator _mediator) : WeatherForecastServiceBase, IPresentationService
    {
        /// <inheritdoc/>
        [TrustedNetwork]
        public override async Task<DeleteForecastResponse> Delete(DeleteForecastRequest request, ServerCallContext context)
        {
            var query = new DeleteWeatherForecastRequest(DateOnly.FromDateTime(request.Date.ToDateTime()));
            await _mediator.Send(query);
            return new DeleteForecastResponse();
        }
    }
}
