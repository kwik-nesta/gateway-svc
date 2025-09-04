using DiagnosKit.Core.Configurations;
using DiagnosKit.Core.Logging;
using KwikNesta.Gateway.Svc.API.Extensions;
using KwikNesta.Gateway.Svc.API.Middlewares;

SerilogBootstrapper.UseBootstrapLogger();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(o =>
{
    o.Filters.Add<GrpcExceptionFilter>();
});
builder.Services
    .RegisterServices(builder.Configuration, builder.Environment.ApplicationName);
builder.Host.ConfigureSerilogESSink();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();
app.UseMiddlewares();
app.Run();
