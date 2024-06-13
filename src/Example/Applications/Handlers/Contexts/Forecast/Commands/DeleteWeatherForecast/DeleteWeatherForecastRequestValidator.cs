using FluentValidation;

namespace Meta.Example.Applications.Handlers.Contexts.Forecast.Commands.DeleteWeatherForecast
{
    /// <summary>
    /// Валидатор команды <see cref="DeleteWeatherForecastRequest"/>
    /// </summary>
    public class DeleteWeatherForecastRequestValidator : AbstractValidator<DeleteWeatherForecastRequest>
    {
    }
}
