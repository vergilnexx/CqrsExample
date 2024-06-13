using Meta.Common.Hosts.Attributes.TrustedNetwork;
using Meta.Common.Hosts.Features.AppFeatures.Swagger.Filters;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Moq;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Meta.Common.Tests.UnitTests.Hosts.Api.Features.Swagger
{
    /// <summary>
    /// Тесты фильтра заголовков для доверенных сетей.
    /// </summary>
    internal class AddTrustedNetworkHeaderFilterTests
    {
        [TrustedNetwork]
        public void TestMethodValidate() { }
        public void TestMethod() { }

        [Test(Description = "При применении фильтра для метода без атрибута, в параметрах операции должно остаться пусто")]
        public void Apply_MethodWithoutAttribute_OperationParametersIsNotEmpty()
        {
            // Arrange
            var filter = new AddTrustedNetworkHeaderFilter();
            var operation = new OpenApiOperation();

            var apiDescription = new ApiDescription();
            var schemaRegistry = new Mock<ISchemaGenerator>();
            var schemaRepository = new SchemaRepository();
            var methodInfo = typeof(AddTrustedNetworkHeaderFilterTests).GetMethod("TestMethod");

            var context = new OperationFilterContext(apiDescription, schemaRegistry.Object, schemaRepository, methodInfo);

            // Act
            filter.Apply(operation, context);

            // Assert
            Assert.That(operation.Parameters, Is.Empty);
        }

        [Test(Description = "При применении фильтра для метода с атрибутом не должно быть исключений, в параметрах операции должны быть записи")]
        public void Apply_MethodWithAttribute_OperationParametersIsNotEmpty()
        {
            // Arrange
            var filter = new AddTrustedNetworkHeaderFilter();
            var operation = new OpenApiOperation();

            var apiDescription = new ApiDescription();
            var schemaRegistry = new Mock<ISchemaGenerator>();
            var schemaRepository = new SchemaRepository();
            var methodInfo = typeof(AddTrustedNetworkHeaderFilterTests).GetMethod("TestMethodValidate");

            var context = new OperationFilterContext(apiDescription, schemaRegistry.Object, schemaRepository, methodInfo);

            // Act
            filter.Apply(operation, context);

            // Assert
            Assert.That(operation.Parameters, Is.Not.Empty);
        }
    }
}
