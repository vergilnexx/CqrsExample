using FluentValidation;
using Meta.Example.Applications.Handlers.Contexts.Forecast.Queries.GetWeatherForecast;

namespace Meta.Example.Applications.Handlers.Contexts.Forecast.Queries.GetWeatherForecastDate
{
    /// <summary>
    /// Валидатор запроса <see cref="GetWeatherForecastRequest"/>
    /// </summary>
    public class GetWeatherForecastDateRequestValidator : AbstractValidator<GetWeatherForecastDateRequest>
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        public GetWeatherForecastDateRequestValidator()
        {
            RuleFor(r => r.Date)
                .NotEmpty()
                .WithMessage("Дата должна быть заполнена");
        }
    }
}
