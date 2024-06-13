using Meta.Common.Hosts.Api.Features.AppFeatures.ExceptionHandler;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Moq;

namespace Meta.Common.Tests.UnitTests.Hosts.Api.Features.ExceptionHandler
{
    /// <summary>
    /// Тесты функциональности обработки исключений.
    /// </summary>
    internal class ExceptionHandlerFeatureTests
    {
        [Test(Description = "Использование фичи, должно происходить без выбрасывания исключения")]
        public void UseFeature_Valid_NoException()
        {
            // Arrange
            var feature = new ExceptionHandlerFeature();
            var builder = new Mock<IApplicationBuilder>();
            var environment = new Mock<IWebHostEnvironment>();

            // Act
            // Assert
            Assert.DoesNotThrow(() =>
            {
                feature.UseFeature(builder.Object, environment.Object);
            });
        }
    }
}
