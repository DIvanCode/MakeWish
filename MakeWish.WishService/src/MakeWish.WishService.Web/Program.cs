using MakeWish.WishService.Adapters.DataAccess.Neo4j;
using MakeWish.WishService.Adapters.MessageBus.RabbitMQ;
using MakeWish.WishService.UseCases;
using MakeWish.WishService.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.SetupUseCases();
builder.Services.SetupDataAccessNeo4j(builder.Configuration);
builder.Services.SetupMessageBusRabbit(builder.Configuration);
builder.Services.SetupWeb(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
