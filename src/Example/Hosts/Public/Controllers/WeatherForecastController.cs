using MediatR;
using Meta.Example.Applications.Handlers.Contexts.Forecast.Commands.AddWeatherForecast;
using Meta.Example.Applications.Handlers.Contexts.Forecast.Commands.UpdateTemperature;
using Meta.Example.Applications.Handlers.Contexts.Forecast.Queries.GetWeatherForecast;
using Meta.Example.Applications.Handlers.Contexts.Forecast.Queries.GetWeatherForecastDate;
using Meta.Example.Contracts.Forecast.Requests;
using Meta.Example.Contracts.Forecast.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Meta.Public.Hosts.Controllers
{
    /// <summary>
    /// Контроллер для работы с предсказаниями погоды.
    /// </summary>
    /// <param name="_mediator">Обработчик сообщений.</param>
    [ApiController]
    [Route("forecast/weather")]
    public class WeatherForecastController(IMediator _mediator) : ControllerBase
    {
        /// <summary>
        /// Возвращает данные о предсказанной погоде.
        /// </summary>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Данные о предсказанной погоде.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<WeatherForecastDto>), statusCode: 200)]
        [ProducesResponseType(typeof(IActionResult), statusCode: 401)]
        [AllowAnonymous]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var request = new GetWeatherForecastRequest();
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        /// <summary>
        /// Возвращает данные о предсказанной погоде в конкретную дату.
        /// </summary>
        /// <param name="date">Дата.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <example>2024-06-11T00:00:00Z</example>
        /// <returns>Данные о предсказанной погоде в конкретную дату.</returns>
        [HttpGet("{date}")]
        [ProducesResponseType(typeof(WeatherForecastDto), statusCode: 200)]
        [Authorize]
        public async Task<IActionResult> Get([FromRoute] DateOnly date, CancellationToken cancellationToken)
        {
            var request = new GetWeatherForecastDateRequest(date);
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        /// <summary>
        /// Добавляет данные о предсказанной погоде.
        /// </summary>
        /// <param name="data">Данные.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Идентификатор добавленной записи.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(int), statusCode: 200)]
        [AllowAnonymous]
        public async Task<IActionResult> Add([FromBody] WeatherForecastData data, CancellationToken cancellationToken)
        {
            var request = new AddWeatherForecastRequest(data);
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        /// <summary>
        /// Обновляет температуру прогноза в конкретную дату.
        /// </summary>
        /// <param name="date">Дата. Пример: 2024-06-11T00:00:00Z</param>
        /// <param name="temperatureC">Температура.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        [HttpPut("{date}/temperature/{temperatureC}")]
        [Authorize]
        public async Task<IActionResult> UpdateTemperature([FromRoute] DateOnly date, [FromRoute] int temperatureC, CancellationToken cancellationToken)
        {
            var request = new UpdateTemperatureWeatherForecastRequest(date, temperatureC);
            await _mediator.Send(request, cancellationToken);
            return Ok();
        }
    }
}
