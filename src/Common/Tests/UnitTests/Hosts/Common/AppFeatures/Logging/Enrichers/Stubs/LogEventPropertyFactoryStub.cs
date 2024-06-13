using Moq;
using Serilog.Core;
using Serilog.Events;

namespace Meta.Common.Test.UnitTests.Hosts.Common.AppFeatures.Logging.Enrichers.Stubs
{
    /// <inheritdoc/>
    internal class LogEventPropertyFactoryStub : ILogEventPropertyFactory
    {
        /// <inheritdoc/>
        public LogEventProperty CreateProperty(string name, object? value, bool destructureObjects = false)
        {
            var mock = new Mock<LogEventPropertyValue>();
            mock.Setup(m => m.ToString()).Returns(value?.ToString() ?? string.Empty);
            return new LogEventProperty(name, mock.Object);
        }
    }
}
