using Meta.Common.Contracts.Options;
using Meta.Common.Hosts.Api.Features.AppFeatures.Authorization;
using Meta.Common.Hosts.Api.Features.AppFeatures.Authorization.Instances.Keycloak;
using Meta.Common.Hosts.Api.Features.AppFeatures.Authorization.Instances.Keycloak.Options;
using Meta.Common.Hosts.Features.AppFeatures.Authorization.Instances.TrustedNetwork;
using Meta.Common.Hosts.Features.AppFeatures.Base;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Reflection;

namespace Meta.Common.Tests.UnitTests.Hosts.Api.Features.Authorization
{
    /// <summary>
    /// Тесты функциональности авторизации.
    /// </summary>
    internal class AuthorizationFeatureTests
    {
        public class TestAuthenticationHandler : IAuthenticationHandler
        {
            public Task<AuthenticateResult> AuthenticateAsync()
            {
                throw new NotImplementedException();
            }

            public Task ChallengeAsync(AuthenticationProperties? properties)
            {
                throw new NotImplementedException();
            }

            public Task ForbidAsync(AuthenticationProperties? properties)
            {
                throw new NotImplementedException();
            }

            public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
            {
                throw new NotImplementedException();
            }
        }

        [Test(Description = "Если настройки не заданы, то коллекция сервисов не содержит обработчики авторизации")]
        public void AddFeature_OptionSectionIsNull_ServiceCollectionsNotContainHandlers()
        {
            // Arrange
            var keys = new Dictionary<string, string?>()
            {
                { "Features", string.Empty },
                { "Features:Authorization", string.Empty },
                { "Features:Authorization:Sections", string.Empty },
                { "Features:Authorization:Sections:0", string.Empty }
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var assembly = new Mock<Assembly>();
            var initRequest = new AppFeatureInitRequest()
            {
                Order = 0,
                Name = "Authorization",
                Configuration = configuration,
                OptionSection = null,
                AdditionalAssemblies = [assembly.Object]
            };
            var feature = new AuthorizationFeature();
            feature.Init(initRequest);
            var services = new ServiceCollection();
            var hostBuilder = new Mock<IHostBuilder>();
            var loggingBuilder = new Mock<ILoggingBuilder>();

            // Act
            feature.AddFeature(services, hostBuilder.Object, loggingBuilder.Object);

            // Assert
            Assert.That(services.Any(s => s.ServiceType == typeof(TrustedNetworkAuthorizationHandler)), Is.False);
            Assert.That(services.Any(s => s.ServiceType == typeof(JwtBearerHandler)), Is.False);
        }

        [Test(Description = "Если настройки валидные, то коллекция сервисов не пустая")]
        public void AddFeature_ValidOptions_ServiceCollectionsIsNotEmpty()
        {
            // Arrange
            var keys = new Dictionary<string, string?>()
            {
                { "Features", string.Empty },
                { "Features:Authorization", string.Empty },
                { "Features:Authorization:Sections", string.Empty },
                { "Features:Authorization:Sections:0", string.Empty }
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var assembly = new Mock<Assembly>();
            var initRequest = new AppFeatureInitRequest()
            {
                Order = 0,
                Name = "Authorization",
                Configuration = configuration,
                OptionSection = configuration.GetSection("Features:Authorization"),
                AdditionalAssemblies = [assembly.Object]
            };
            var feature = new AuthorizationFeature();
            feature.Init(initRequest);
            var services = new ServiceCollection();
            var hostBuilder = new Mock<IHostBuilder>();
            var loggingBuilder = new Mock<ILoggingBuilder>();

            // Act
            feature.AddFeature(services, hostBuilder.Object, loggingBuilder.Object);

            // Assert
            Assert.That(services, Is.Not.Empty);
        }

        [Test(Description = "Если авторизация по доверенным сетям, то коллекция сервисов не пустая")]
        public void AddFeature_TrustedNetwork_ServiceCollectionsIsNotEmpty()
        {
            // Arrange
            var keys = new Dictionary<string, string?>()
            {
                { "Features", string.Empty },
                { "Features:Authorization", string.Empty },
                { "Features:Authorization:Sections", string.Empty },
                { "Features:Authorization:Sections:0", string.Empty },
                { "Features:Authorization:Sections:0:TrustedNetwork", string.Empty },
                { "Features:Authorization:Sections:0:TrustedNetwork:TrustedNetworks", "::1/128; 127.0.0.0/8" }
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var assembly = new Mock<Assembly>();
            var initRequest = new AppFeatureInitRequest()
            {
                Order = 0,
                Name = "Authorization",
                Configuration = configuration,
                OptionSection = configuration.GetSection("Features:Authorization"),
                AdditionalAssemblies = [assembly.Object]
            };
            var feature = new AuthorizationFeature();
            feature.Init(initRequest);
            var services = new ServiceCollection();
            var hostBuilder = new Mock<IHostBuilder>();
            var loggingBuilder = new Mock<ILoggingBuilder>();

            // Act
            feature.AddFeature(services, hostBuilder.Object, loggingBuilder.Object);

            // Assert
            Assert.That(services.Any(s => s.ServiceType == typeof(TrustedNetworkAuthorizationHandler)), Is.True);
        }

        [Test(Description = "Если авторизация по доверенным сетям, то коллекция сервисов не пустая")]
        public void AddFeature_Keycloak_ServiceCollectionsIsNotEmpty()
        {
            // Arrange
            var keys = new Dictionary<string, string?>()
            {
                { "Features", string.Empty },
                { "Features:Authorization", string.Empty },
                { "Features:Authorization:Sections", string.Empty },
                { "Features:Authorization:Sections:0", string.Empty },
                { "Features:Authorization:Sections:0:Keycloak", string.Empty },
                { "Features:Authorization:Sections:0:Keycloak:Authority", "http://localhost" },
                { "Features:Authorization:Sections:0:Keycloak:ClientId", "ue-signaling" },
                { "Features:Authorization:Sections:0:Keycloak:MetadataAddress", "http://localhost" }
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var assembly = new Mock<Assembly>();
            var initRequest = new AppFeatureInitRequest()
            {
                Order = 0,
                Name = "Authorization",
                Configuration = configuration,
                OptionSection = configuration.GetSection("Features:Authorization"),
                AdditionalAssemblies = [assembly.Object]
            };
            var feature = new AuthorizationFeature();
            feature.Init(initRequest);
            var services = new ServiceCollection();
            var hostBuilder = new Mock<IHostBuilder>();
            var loggingBuilder = new Mock<ILoggingBuilder>();

            // Act
            feature.AddFeature(services, hostBuilder.Object, loggingBuilder.Object);

            // Assert
            Assert.That(services.Any(s => s.ServiceType == typeof(JwtBearerHandler)), Is.True);
        }

        [Test(Description = "Использование фичи, должно происходить без выбрасывания исключения")]
        public void UseFeature_Valid_NoException()
        {
            // Arrange
            var keys = new Dictionary<string, string?>()
            {
                { "Features", string.Empty },
                { "Features:Authorization", string.Empty },
                { "Features:Authorization:Sections", string.Empty },
                { "Features:Authorization:Sections:0", string.Empty }
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var assembly = new Mock<Assembly>();
            var initRequest = new AppFeatureInitRequest()
            {
                Order = 0,
                Name = "Authorization",
                Configuration = configuration,
                OptionSection = null,
                AdditionalAssemblies = [assembly.Object]
            };
            var feature = new AuthorizationFeature();
            feature.Init(initRequest);
            var services = new ServiceCollection();
            var hostBuilder = new Mock<IHostBuilder>();
            var loggingBuilder = new Mock<ILoggingBuilder>();

            var environment = new Mock<IWebHostEnvironment>();
            feature.AddFeature(services, hostBuilder.Object, loggingBuilder.Object);
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            var builder = new ApplicationBuilder(serviceProvider);

            // Act
            // Assert
            Assert.DoesNotThrow(() =>
            {
                feature.UseFeature(builder, environment.Object);
            });
        }

        [Test(Description = "При регистрации Keycloak авторизации, в настройки записывается Authority")]
        public void GetJwtBearerSettings_ValidOptions_AuthorityFilled()
        {
            // Arrange
            var keys = new Dictionary<string, string?>()
            {
                { "Features", string.Empty },
                { "Features:Authorization", string.Empty },
                { "Features:Authorization:Sections", string.Empty },
                { "Features:Authorization:Sections:0", string.Empty },
                { "Features:Authorization:Sections:0:Keycloak", string.Empty },
                { "Features:Authorization:Sections:0:Keycloak:Authority", "http://localhost" },
                { "Features:Authorization:Sections:0:Keycloak:ClientId", "ue-signaling" },
                { "Features:Authorization:Sections:0:Keycloak:MetadataAddress", "http://localhost" }
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var optionSection = configuration.GetSection("Features:Authorization:Sections:0");
            var keycloakAuthorizationOptions = optionSection.GetSection(KeycloakAuthorizationOptions.Scheme).Get<KeycloakAuthorizationOptions>();
            var options = new JwtBearerOptions();

            // Act
            KeycloakAuthorizationFeatureExtension.GetJwtBearerSettings(options, keycloakAuthorizationOptions!);

            // Assert
            Assert.That(options.Authority, Is.EqualTo("http://localhost"));
        }
        
        [Test(Description = "При регистрации Keycloak авторизации, в настройки записывается Audience")]
        public void GetJwtBearerSettings_ValidOptions_AudienceFilled()
        {
            // Arrange
            var keys = new Dictionary<string, string?>()
            {
                { "Features", string.Empty },
                { "Features:Authorization", string.Empty },
                { "Features:Authorization:Sections", string.Empty },
                { "Features:Authorization:Sections:0", string.Empty },
                { "Features:Authorization:Sections:0:Keycloak", string.Empty },
                { "Features:Authorization:Sections:0:Keycloak:Authority", "http://localhost" },
                { "Features:Authorization:Sections:0:Keycloak:ClientId", "ue-signaling" },
                { "Features:Authorization:Sections:0:Keycloak:MetadataAddress", "http://localhost" }
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var optionSection = configuration.GetSection("Features:Authorization:Sections:0");
            var keycloakAuthorizationOptions = optionSection.GetSection(KeycloakAuthorizationOptions.Scheme).Get<KeycloakAuthorizationOptions>();
            var options = new JwtBearerOptions();

            // Act
            KeycloakAuthorizationFeatureExtension.GetJwtBearerSettings(options, keycloakAuthorizationOptions!);

            // Assert
            Assert.That(options.Audience, Is.EqualTo("ue-signaling"));
        }

        [Test(Description = "При регистрации Keycloak авторизации, в настройки записывается RequireHttpsMetadata")]
        public void GetJwtBearerSettings_ValidOptions_RequireHttpsMetadataFilled()
        {
            // Arrange
            var keys = new Dictionary<string, string?>()
            {
                { "Features", string.Empty },
                { "Features:Authorization", string.Empty },
                { "Features:Authorization:Sections", string.Empty },
                { "Features:Authorization:Sections:0", string.Empty },
                { "Features:Authorization:Sections:0:Keycloak", string.Empty },
                { "Features:Authorization:Sections:0:Keycloak:Authority", "http://localhost" },
                { "Features:Authorization:Sections:0:Keycloak:ClientId", "ue-signaling" },
                { "Features:Authorization:Sections:0:Keycloak:RequireHttpsMetadata", true.ToString() },
                { "Features:Authorization:Sections:0:Keycloak:MetadataAddress", "http://localhost" }
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var optionSection = configuration.GetSection("Features:Authorization:Sections:0");
            var keycloakAuthorizationOptions = optionSection.GetSection(KeycloakAuthorizationOptions.Scheme).Get<KeycloakAuthorizationOptions>();
            var options = new JwtBearerOptions();

            // Act
            KeycloakAuthorizationFeatureExtension.GetJwtBearerSettings(options, keycloakAuthorizationOptions!);

            // Assert
            Assert.That(options.RequireHttpsMetadata, Is.EqualTo(true));
        }

        [Test(Description = "При регистрации Keycloak авторизации, в настройки записывается MetadataAddress")]
        public void GetJwtBearerSettings_ValidOptions_MetadataAddressFilled()
        {
            // Arrange
            var keys = new Dictionary<string, string?>()
            {
                { "Features", string.Empty },
                { "Features:Authorization", string.Empty },
                { "Features:Authorization:Sections", string.Empty },
                { "Features:Authorization:Sections:0", string.Empty },
                { "Features:Authorization:Sections:0:Keycloak", string.Empty },
                { "Features:Authorization:Sections:0:Keycloak:Authority", "http://localhost" },
                { "Features:Authorization:Sections:0:Keycloak:ClientId", "ue-signaling" },
                { "Features:Authorization:Sections:0:Keycloak:MetadataAddress", "http://localhost" }
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var optionSection = configuration.GetSection("Features:Authorization:Sections:0");
            var keycloakAuthorizationOptions = optionSection.GetSection(KeycloakAuthorizationOptions.Scheme).Get<KeycloakAuthorizationOptions>();
            var options = new JwtBearerOptions();

            // Act
            KeycloakAuthorizationFeatureExtension.GetJwtBearerSettings(options, keycloakAuthorizationOptions!);

            // Assert
            Assert.That(options.MetadataAddress, Is.EqualTo("http://localhost"));
        }

        [Test(Description = "При регистрации Keycloak авторизации если настройки неопрделенны, Authority не заполняется")]
        public void GetJwtBearerSettings_KeycloakOptionsNull_AuthorityIsNull()
        {
            // Arrange
            KeycloakAuthorizationOptions? keycloakAuthorizationOptions = null;
            var options = new JwtBearerOptions();

            // Act
            KeycloakAuthorizationFeatureExtension.GetJwtBearerSettings(options, keycloakAuthorizationOptions!);

            // Assert
            Assert.That(options.Authority, Is.Null);
        }

        [Test(Description = "При регистрации Keycloak авторизации, в настройки записывается MetadataAddress")]
        public void AuthenticationFailed_ValidOptions_MetadataAddressFilled()
        {
            // Arrange
            var keys = new Dictionary<string, string?>()
            {
                { "Features", string.Empty },
                { "Features:Authorization", string.Empty },
                { "Features:Authorization:Sections", string.Empty },
                { "Features:Authorization:Sections:0", string.Empty },
                { "Features:Authorization:Sections:0:Keycloak", string.Empty },
                { "Features:Authorization:Sections:0:Keycloak:Authority", "http://localhost" },
                { "Features:Authorization:Sections:0:Keycloak:ClientId", "ue-signaling" },
                { "Features:Authorization:Sections:0:Keycloak:MetadataAddress", "http://localhost" }
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var optionSection = configuration.GetSection("Features:Authorization:Sections:0");
            var keycloakAuthorizationOptions = optionSection.GetSection(KeycloakAuthorizationOptions.Scheme).Get<KeycloakAuthorizationOptions>();
            var options = new JwtBearerOptions();
            var context = new DefaultHttpContext();
            var scheme = new AuthenticationScheme("name", "displayName",typeof(TestAuthenticationHandler));

            // Act
            KeycloakAuthorizationFeatureExtension.GetJwtBearerSettings(options, keycloakAuthorizationOptions!);
            options.Events.AuthenticationFailed(new AuthenticationFailedContext(context, scheme, options) { Exception = new Exception("ошибка") });

            // Assert
            Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status401Unauthorized));
        }

        [Test(Description = "При регистрации Keycloak авторизации если настройки определенны, они должны быть смапленны")]
        public void Configure_KeycloakOptionsIsNotEmpty_OptionsIsNotNull()
        {
            // Arrange
            var keys = new Dictionary<string, string?>()
            {
                { "Features", string.Empty },
                { "Features:Authorization", string.Empty },
                { "Features:Authorization:Sections", string.Empty },
                { "Features:Authorization:Sections:0", string.Empty },
                { "Features:Authorization:Sections:0:Keycloak", string.Empty },
                { "Features:Authorization:Sections:0:Keycloak:Authority", "http://localhost" },
                { "Features:Authorization:Sections:0:Keycloak:ClientId", "ue-signaling" },
                { "Features:Authorization:Sections:0:Keycloak:MetadataAddress", "http://localhost" }
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var optionSection = configuration.GetSection("Features:Authorization:Sections:0");
            var services = new ServiceCollection();

            // Act
            services.AddKeycloakAuthenticate(optionSection);
            var provider = services.BuildServiceProvider();
            var keycloakAuthorizationOptions = provider.GetService<IOptions<KeycloakAuthorizationOptions>>();

            // Assert
            Assert.That(keycloakAuthorizationOptions!.Value, Is.Not.Null);
        }

        [Test(Description = "При регистрации Keycloak авторизации если настройки определенны, они должны быть смапленны")]
        public void Configure_TrustedNetworkOptionsIsNotEmpty_OptionsIsNotNull()
        {
            // Arrange
            var keys = new Dictionary<string, string?>()
            {
                { "Features", string.Empty },
                { "Features:Authorization", string.Empty },
                { "Features:Authorization:Sections", string.Empty },
                { "Features:Authorization:Sections:0", string.Empty },
                { "Features:Authorization:Sections:0:TrustedNetwork", string.Empty },
                { "Features:Authorization:Sections:0:TrustedNetwork:TrustedNetworks", "::1/128; 127.0.0.0/8" }
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(keys).Build();
            var optionSection = configuration.GetSection("Features:Authorization:Sections:0");
            var services = new ServiceCollection();

            // Act
            services.AddTrustedNetworkAuthenticate(optionSection);
            var provider = services.BuildServiceProvider();
            var trustedNetworkAuthorizationOptions = provider.GetService<IOptions<TrustedNetworkOptions>>();

            // Assert
            Assert.That(trustedNetworkAuthorizationOptions!.Value, Is.Not.Null);
        }
    }
}
