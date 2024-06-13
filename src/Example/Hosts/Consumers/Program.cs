using Meta.Common.Hosts.Consumer;
using Meta.Common.Hosts.Consumer.Extensions;
using Meta.Common.Hosts.Features.AppFeatures.Base;
using Meta.Example.Clients.Options;
using Meta.Example.Hosts.Consumer.Consumers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Meta.Example.Clients;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingletonWeatherForecastServiceClient(builder.Configuration);

builder.Services.AddOptions<WeatherForecastClientOptions>()
                .Configure<IConfiguration>((options, configuration) =>
                {
                    var section = configuration.GetSection(WeatherForecastClientOptions.SectionName);
                    if (!section.Exists())
                    {
                        throw new InvalidOperationException(
                            $"Не удалось получить настройки секции '{WeatherForecastClientOptions.SectionName}'.");
                    }
                    section.Bind(options);
                });

builder.Services
       .RegisterConsumer<DeleteWeatherForecastConsumer>()
       .RegisterConsumer<WeatherForecastAddedNotificationConsumer>()
       .RegisterRabbitMqService()
       .AddFeatures(builder, builder.Logging, builder.Configuration,
        [
            Assembly.GetExecutingAssembly(),
            typeof(IAppFeature).Assembly,
            typeof(HostRegistrar).Assembly
        ]);

builder.Build().Run();