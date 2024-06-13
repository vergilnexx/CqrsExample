using Meta.Common.Applications.AppServices.Contexts.Common.Services.DiagnosticProvider;
using Meta.Common.Applications.Handlers.Abstract;
using System.Diagnostics;
using System.Reflection;

namespace Meta.Example.Applications.Handlers.Contexts.Forecast.Queries.GetWeatherForecast
{
    /// <summary>
    /// Обработчик диагностики запроса <see cref=" GetWeatherForecastRequest"/>
    /// </summary>
    public class GetWeatherForecastRequestDiagnosticHandler : IDiagnosticHandler<GetWeatherForecastRequest>
    {
        private readonly IDiagnosticProvider _diagnosticProvider;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="diagnosticProvider">Провайдер работы с диагностикой.</param>
        public GetWeatherForecastRequestDiagnosticHandler(IDiagnosticProvider diagnosticProvider)
        {
            _diagnosticProvider = diagnosticProvider;
        }

        /// <inheritdoc/>
        public Activity? PreHandle(GetWeatherForecastRequest request)
        {
            var activity = _diagnosticProvider.StartActivity(Assembly.GetEntryAssembly());
            activity?.SetTag("correlationId", request.CorrelationId.ToString());

            return activity;
        }

        /// <inheritdoc/>
        public void PostHandle(Activity? actiity, GetWeatherForecastRequest request)
        {
            var counter = _diagnosticProvider.CreateCounter<int>("get.count", "Количество добавленных прогнозов");
            counter?.Add(1);
        }
    }
}
