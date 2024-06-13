using Meta.Common.Contracts.Exceptions.Common;
using Meta.Example.Infrastructures.DataAccess.Contexts;
using Meta.Example.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

var app = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var connectionString = hostContext.Configuration.GetConnectionString("ExampleDb");

        services.AddDbContext<ExampleDbContext>(dbContextBuilder =>
            dbContextBuilder.UseNpgsql(connectionString, b => b.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName)));

        services.AddSingleton<Startup>();
    }).Build();

var migration = app.Services.GetService<Startup>()
                    ?? throw new NotFoundException("Не удалось получить контекст доступа к базе данных");

using var cancellationTokenSource = new CancellationTokenSource();

await migration.StartMigrationsAsync(cancellationTokenSource.Token);
