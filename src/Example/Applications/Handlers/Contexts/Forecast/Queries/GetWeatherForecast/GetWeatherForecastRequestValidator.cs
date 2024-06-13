using FluentValidation;

namespace Meta.Example.Applications.Handlers.Contexts.Forecast.Queries.GetWeatherForecast
{
    /// <summary>
    /// Валидатор запроса <see cref="GetWeatherForecastRequest"/>
    /// </summary>
    public class GetWeatherForecastRequestValidator : AbstractValidator<GetWeatherForecastRequest>
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        public GetWeatherForecastRequestValidator()
        {
        }
    }
}
