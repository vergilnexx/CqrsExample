namespace Meta.Common.Hosts.Api.Features.AppFeatures.OpenTelemetry
{
    /// <summary>
    /// Настройки OpenTelemetry
    /// </summary>
    internal class OpenTelemetryFeatureOptions
    {
        /// <summary>
        /// Наименование приложения.
        /// </summary>
        public required string ApplicationName { get; set; }

        /// <summary>
        /// Признак, что включен Prometheus
        /// </summary>
        public bool PrometheusEnabled { get; set; }

        /// <summary>
        /// URL экспортера по протоколу OTLP.
        /// </summary>
        public required string ExporterOtlpUrl { get; set; }
    }
}
