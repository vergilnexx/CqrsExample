using MediatR;
using Meta.Common.Hosts.Attributes.TrustedNetwork;
using Meta.Example.Applications.Handlers.Contexts.Forecast.Commands.DeleteWeatherForecast;
using Meta.Example.Applications.Handlers.Contexts.Forecast.Queries.GetWeatherForecastDate;
using Meta.Example.Contracts.Forecast.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Meta.Example.Private.Hosts.Api.Controllers
{
    /// <summary>
    /// Контроллер для работы с предсказаниями погоды.
    /// </summary>
    /// <param name="_mediator">Обработчик сообщений.</param>
    [ApiController]
    [Route("forecast/weather")]
    [TrustedNetwork]
    public class WeatherForecastController(IMediator _mediator) : ControllerBase
    {
        /// <summary>
        /// Удаляет прогноз на конкретную дату.
        /// </summary>
        /// <param name="date">Дата. Пример: 2024-06-11T00:00:00Z</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        [HttpDelete("{date}")]
        public async Task<IActionResult> Delete([FromRoute] DateOnly date, CancellationToken cancellationToken)
        {
            var request = new DeleteWeatherForecastRequest(date);
            await _mediator.Send(request, cancellationToken);
            return Ok();
        }
    }
}
