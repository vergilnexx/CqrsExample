using Meta.Example.Private.Hosts.Grpc;
using Meta.Common.Hosts.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegistrarServices(builder.Host, builder.Logging, builder.Configuration);

var app = builder.Build();

app.UseLocalization();
app.UseFeatures(app.Environment);

app.MapServices();

app.Run();