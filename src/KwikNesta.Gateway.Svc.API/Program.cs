using DiagnosKit.Core.Configurations;
using DiagnosKit.Core.Logging;
using DiagnosKit.Core.Logging.Contracts;
using DiagnosKit.Core.Middlewares;
using KwikNesta.Gateway.Svc.API.Extensions;

SerilogBootstrapper.UseBootstrapLogger();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services
    .RegisterServices(builder.Configuration, builder.Environment.ApplicationName);
builder.Host.ConfigureESSink(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILoggerManager>();
app.UseDiagnosKitExceptionHandler(logger);
app.UseMiddlewares(builder.Configuration);

app.Run();
