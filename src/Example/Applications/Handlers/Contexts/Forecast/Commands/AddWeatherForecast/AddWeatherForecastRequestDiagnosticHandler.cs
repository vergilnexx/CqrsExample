using Meta.Common.Applications.AppServices.Contexts.Common.Services.DiagnosticProvider;
using Meta.Common.Applications.Handlers.Abstract;
using System.Diagnostics;
using System.Reflection;

namespace Meta.Example.Applications.Handlers.Contexts.Forecast.Commands.AddWeatherForecast
{
    /// <summary>
    /// Обработчик диагностики запроса <see cref="AddWeatherForecastRequest"/>
    /// </summary>
    public class AddWeatherForecastRequestDiagnosticHandler : IDiagnosticHandler<AddWeatherForecastRequest, int>
    {
        private readonly IDiagnosticProvider _diagnosticProvider;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="diagnosticProvider">Провайдер работы с диагностикой.</param>
        public AddWeatherForecastRequestDiagnosticHandler(IDiagnosticProvider diagnosticProvider)
        {
            _diagnosticProvider = diagnosticProvider;
        }

        /// <inheritdoc/>
        public Activity? PreHandle(AddWeatherForecastRequest request)
        {
            var activity = _diagnosticProvider.StartActivity(Assembly.GetEntryAssembly());
            activity?.SetTag("correlationId", request.CorrelationId.ToString());

            return activity;
        }

        /// <inheritdoc/>
        public void PostHandle(Activity? actiity, AddWeatherForecastRequest request, int response)
        {
            var counter = _diagnosticProvider.CreateCounter<int>("added.count", "Количество добавленных прогнозов");
            counter?.Add(1);
        }
    }
}
