using MediatR;
using Meta.Common.Applications.Handlers.Abstract;
using Meta.Common.Applications.Handlers.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using System.Reflection;
using static Meta.Common.Tests.UnitTests.Cqrs.Behaviors.EventSendingBehaviorTests;

namespace Meta.Common.Test.UnitTests.Handlers.Extensions
{
    /// <summary>
    /// ����� ����������� ������������.
    /// </summary>
    internal class HandlerRegistrarTests
    {
        [Test(Description = "���� ������ �� ��������, �� ������ ���� ��������� ����������.")]
        public void AddValidators_AssemblyNull_ArgumentNullExceptionThrowed()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                services.AddValidators(assembly: null);
            });

            // Assert
            Assert.That(exception, Is.Not.Null);
        }

        [Test(Description = "���� � ������ ��� �� ������ ����������, �� ��������� ������������������ �������� ������ ���� ������.")]
        public void AddValidators_AssemblyNotContainValidators_CollectionIsEmpty()
        {
            // Arrange
            var services = new ServiceCollection();
            var assembly = typeof(HandlerRegistrar).Assembly;

            // Act
            services.AddValidators(assembly);

            // Assert
            Assert.That(services, Is.Empty);
        }

        [Test(Description = "���� ������ �� ��������, �� ������ ���� ��������� ����������.")]
        public void AddRequestHandlers_AssemblyNull_ArgumentNullExceptionThrowed()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                services.AddRequestHandlers(assembly: null);
            });

            // Assert
            Assert.That(exception, Is.Not.Null);
        }

        [Test(Description = "���� � ������ ��� �� ������ ������������, �� ��������� ������������������ �������� ������ ���� ������.")]
        public void AddRequestHandlers_AssemblyNotContainHandlers_CollectionIsEmpty()
        {
            // Arrange
            var services = new ServiceCollection();
            var assembly = new Mock<Assembly>();

            // Act
            services.AddRequestHandlers(assembly.Object);

            // Assert
            Assert.That(services, Is.Empty);
        }

        [Test(Description = "���� � ������ ���� ���� ���������� �������, �� � ��������� ������������������ �������� ������ ���� ���� ������.")]
        public void AddRequestHandlers_AssemblyContainCommandHandler_CollectionContainOneElement()
        {
            // Arrange
            var services = new ServiceCollection();
            var assembly = new Mock<Assembly>();
            var commandInterface = new Mock<Type>();
            var requestInterface = new Mock<Type>();
            var type = new Mock<TypeInfo>();
            commandInterface.Setup(t => t.IsGenericType).Returns(true);
            commandInterface.Setup(t => t.GetGenericTypeDefinition()).Returns(typeof(ICommandHandler<>));
            requestInterface.Setup(t => t.IsGenericType).Returns(true);
            requestInterface.Setup(t => t.GetGenericTypeDefinition()).Returns(typeof(IRequestHandler<>));
            type.Setup(t => t.GetInterfaces()).Returns([requestInterface.Object, commandInterface.Object]);
            assembly.Setup(a => a.DefinedTypes).Returns([type.Object]);

            // Act
            services.AddRequestHandlers(assembly.Object);

            // Assert
            Assert.That(services, Has.Count.EqualTo(1));
        }

        [Test(Description = "���� � ������ ���� ���� ���������� ������� � ������������ �����������, �� � ��������� ������������������ �������� ������ ���� ���� ������.")]
        public void AddRequestHandlers_AssemblyContainResultCommandHandler_CollectionContainOneElement()
        {
            // Arrange
            var services = new ServiceCollection();
            var assembly = new Mock<Assembly>();
            var commandInterface = new Mock<Type>();
            var requestInterface = new Mock<Type>();
            var type = new Mock<TypeInfo>();
            commandInterface.Setup(t => t.IsGenericType).Returns(true);
            commandInterface.Setup(t => t.GetGenericTypeDefinition()).Returns(typeof(ICommandHandler<,>));
            requestInterface.Setup(t => t.IsGenericType).Returns(true);
            requestInterface.Setup(t => t.GetGenericTypeDefinition()).Returns(typeof(IRequestHandler<>));
            type.Setup(t => t.GetInterfaces()).Returns([requestInterface.Object, commandInterface.Object]);
            assembly.Setup(a => a.DefinedTypes).Returns([type.Object]);

            // Act
            services.AddRequestHandlers(assembly.Object);

            // Assert
            Assert.That(services, Has.Count.EqualTo(1));
        }

        [Test(Description = "���� � ������ ���� ���� ���������� �������, �� � ��������� ������������������ �������� ������ ���� ���� ������.")]
        public void AddRequestHandlers_AssemblyContainQueryHandler_CollectionContainOneElement()
        {
            // Arrange
            var services = new ServiceCollection();
            var assembly = new Mock<Assembly>();
            var commandInterface = new Mock<Type>();
            var requestInterface = new Mock<Type>();
            var type = new Mock<TypeInfo>();
            commandInterface.Setup(t => t.IsGenericType).Returns(true);
            commandInterface.Setup(t => t.GetGenericTypeDefinition()).Returns(typeof(IQueryHandler<,>));
            requestInterface.Setup(t => t.IsGenericType).Returns(true);
            requestInterface.Setup(t => t.GetGenericTypeDefinition()).Returns(typeof(IRequestHandler<>));
            type.Setup(t => t.GetInterfaces()).Returns([requestInterface.Object, commandInterface.Object]);
            assembly.Setup(a => a.DefinedTypes).Returns([type.Object]);

            // Act
            services.AddRequestHandlers(assembly.Object);

            // Assert
            Assert.That(services, Has.Count.EqualTo(1));
        }

        [Test(Description = "���� ������ �� ��������, �� ������ ���� ��������� ����������.")]
        public void AddAssemblyHandlers_AssemblyNull_ArgumentNullExceptionThrowed()
        {
            // Arrange
            var services = new Mock<IServiceCollection>();

            // Act
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                services.Object.AddAssemblyHandlers(assembly: null);
            });

            // Assert
            Assert.That(exception, Is.Not.Null);
        }

        [Test(Description = "���� ��������� �������� �� ��������, �� ������ ���� ��������� ����������.")]
        public void AddAssemblyHandlers_ServicesNull_ArgumentNullExceptionThrowed()
        {
            // Arrange
            var assembly = new Mock<Assembly>();

            // Act
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                HandlerRegistrar.AddAssemblyHandlers(services: null, assembly.Object);
            });

            // Assert
            Assert.That(exception, Is.Not.Null);
        }

        [Test(Description = "���� � ������ ��� �� ������ �����������, �� � ��������� ������ �� �����������.")]
        public void AddAssemblyHandlers_AssemblyNotContainHandlers_ServicesNotAdded()
        {
            // Arrange
            var services = new Mock<IServiceCollection>();
            var assembly = new Mock<Assembly>();

            // Act
            HandlerRegistrar.AddAssemblyHandlers(services.Object, assembly.Object);

            // Assert
            services.Verify(s => s.Add(It.IsAny<ServiceDescriptor>()), Times.Never);
        }

        [Test(Description = "���� � ������ ���� ���� ���������� �����������, �� � ��������� ������������������ �������� ������ ���� ���� ������.")]
        public void AddDiagnosticHandlers_AssemblyContainQueryHandler_CollectionContainOneElement()
        {
            // Arrange
            var services = new ServiceCollection();
            var assembly = new Mock<Assembly>();
            var diagnosticInterface = new Mock<Type>();
            var type = new Mock<TypeInfo>();
            diagnosticInterface.Setup(t => t.IsGenericType).Returns(true);
            diagnosticInterface.Setup(t => t.GetGenericTypeDefinition()).Returns(typeof(IDiagnosticHandler<,>));
            type.Setup(t => t.GetInterfaces()).Returns([diagnosticInterface.Object]);
            assembly.Setup(a => a.DefinedTypes).Returns([type.Object]);

            // Act
            services.AddDiagnosticHandlers(assembly.Object);

            // Assert
            Assert.That(services, Has.Count.EqualTo(1));
        }

        [Test(Description = "���� � ������ ���� ���� ���������� �������, �� � ��������� ������������������ �������� ������ ���� ���� ������.")]
        public void AddEventHandlers_AssemblyContainQueryHandler_CollectionContainOneElement()
        {
            // Arrange
            var services = new ServiceCollection();
            var assembly = new Mock<Assembly>();
            var handlerType = new Mock<Type>();
            var type = new Mock<TypeInfo>();
            type.Setup(t => t.GetInterfaces()).Returns([typeof(IEventHandler)]);
            type.SetupGet(t => t.BaseType).Returns(handlerType.Object);
            assembly.Setup(a => a.DefinedTypes).Returns([type.Object]);

            // Act
            services.AddEventHandlers(assembly.Object);

            // Assert
            Assert.That(services, Has.Count.EqualTo(1));
        }
    }
}