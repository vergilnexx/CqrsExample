using Meta.Common.Applications.AppServices.Contexts.Common.Services.DiagnosticProvider;
using Meta.Common.Applications.Handlers.Abstract;
using System.Diagnostics;
using System.Reflection;

namespace Meta.Example.Applications.Handlers.Contexts.Forecast.Commands.DeleteWeatherForecast
{
    /// <summary>
    /// Обработчик диагностики запроса <see cref="DeleteWeatherForecastRequest"/>
    /// </summary>
    public class DeleteWeatherForecastRequestDiagnosticHandler : IDiagnosticHandler<DeleteWeatherForecastRequest>
    {
        private readonly IDiagnosticProvider _diagnosticProvider;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="diagnosticProvider">Провайдер работы с диагностикой.</param>
        public DeleteWeatherForecastRequestDiagnosticHandler(IDiagnosticProvider diagnosticProvider)
        {
            _diagnosticProvider = diagnosticProvider;
        }

        /// <inheritdoc/>
        public Activity? PreHandle(DeleteWeatherForecastRequest request)
        {
            var activity = _diagnosticProvider.StartActivity(Assembly.GetEntryAssembly());
            activity?.SetTag("correlationId", request.CorrelationId.ToString());

            return activity;
        }

        /// <inheritdoc/>
        public void PostHandle(Activity? actiity, DeleteWeatherForecastRequest request)
        {
            var counter = _diagnosticProvider.CreateCounter<int>("added.count", "Количество добавленных прогнозов");
            counter?.Add(-1);
        }
    }
}
