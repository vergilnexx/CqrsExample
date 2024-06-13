using FluentValidation;
using Meta.Common.Applications.AppServices.Contexts.Common.Services.DateTimeProvider;

namespace Meta.Example.Applications.Handlers.Contexts.Forecast.Commands.AddWeatherForecast
{
    /// <summary>
    /// Валидатор запроса <see cref="AddWeatherForecastRequest"/>
    /// </summary>
    public class AddWeatherForecastRequestValidator : AbstractValidator<AddWeatherForecastRequest>
    {
        private readonly static int SummaryMaxLength = 2000;

        /// <summary>
        /// Конструктор.
        /// </summary>
        public AddWeatherForecastRequestValidator(IDateTimeProvider dateTimeProvider)
        {
            RuleFor(r => r.Summary)
                .MaximumLength(SummaryMaxLength)
                .WithMessage($"Описание не может превышать {SummaryMaxLength} символов");
            RuleFor(r => r.Date)
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(dateTimeProvider.UtcNow.Date))
                .WithMessage("Дата должна быть в будущем");
        }
    }
}
