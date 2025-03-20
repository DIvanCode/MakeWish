using MakeWish.UserService.Web.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.SetupWeb(builder.Configuration);
builder.Services.SetupUseCases();
builder.Services.SetupDataAccess();

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