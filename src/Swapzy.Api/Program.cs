using Swapzy.Api.Extensions;
using Swapzy.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwapzyAuth(builder.Configuration);
builder.Services.AddSwapzyDatabase(builder.Configuration);
builder.Services.AddSwapzyServices();
builder.Services.AddSwapzyAws(builder.Configuration);
builder.Services.AddSwapzyPresentation();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
