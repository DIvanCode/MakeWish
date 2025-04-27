using MakeWish.WishService.Adapters.DataAccess.Neo4j;
using MakeWish.WishService.UseCases;
using MakeWish.WishService.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.SetupWeb(builder.Configuration);
builder.Services.SetupUseCases();
builder.Services.SetupDataAccessNeo4j(builder.Configuration);

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
