using MakeWish.UserService.Adapters.DataAccess.EntityFramework;
using MakeWish.UserService.Adapters.MessageBus.RabbitMQ;
using MakeWish.UserService.UseCases;
using MakeWish.UserService.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.SetupWeb(builder.Configuration);
builder.Services.SetupDataAccessEntityFramework(builder.Configuration);
builder.Services.SetupMessageBusRabbit(builder.Configuration);
builder.Services.SetupUseCases();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
        
app.MapControllers();

app.Run();
