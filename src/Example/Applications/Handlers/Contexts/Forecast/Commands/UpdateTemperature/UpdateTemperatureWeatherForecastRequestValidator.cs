using FluentValidation;
using Meta.Common.Applications.AppServices.Contexts.Common.Services.DateTimeProvider;

namespace Meta.Example.Applications.Handlers.Contexts.Forecast.Commands.UpdateTemperature
{
    /// <summary>
    /// Валидатор запроса <see cref="UpdateTemperatureWeatherForecastRequest"/>
    /// </summary>
    public class UpdateTemperatureWeatherForecastRequestValidator : AbstractValidator<UpdateTemperatureWeatherForecastRequest>
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        public UpdateTemperatureWeatherForecastRequestValidator(IDateTimeProvider dateTimeProvider)
        {
            RuleFor(r => r.TemperatureC)
                .NotEmpty()
                .WithMessage("Температура должна быть заполнена");
            RuleFor(r => r.Date)
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(dateTimeProvider.UtcNow.Date))
                .WithMessage("Дата должна быть в будущем");
        }
    }
}
