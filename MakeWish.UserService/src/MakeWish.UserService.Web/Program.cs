using MakeWish.UserService.Adapters.DataAccess.EntityFramework;
using MakeWish.UserService.Adapters.MessageBus.RabbitMQ;
using MakeWish.UserService.Telemetry;
using MakeWish.UserService.UseCases;
using MakeWish.UserService.Web;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);

builder.Services.SetupWeb(builder.Configuration);
builder.Services.SetupDataAccessEntityFramework(builder.Configuration);
builder.Services.SetupMessageBusRabbit(builder.Configuration);
builder.Services.SetupUseCases();
builder.Services.SetupTelemetry(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseOpenTelemetryPrometheusScrapingEndpoint();
        
app.MapControllers();

app.Run();
