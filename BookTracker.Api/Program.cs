using BookTracker.Api.Wiring;

var builder = WebApplication.CreateBuilder(args); // create builder for WebApplication
builder.AddBookTracker();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseCors();
app.UseBookTracker();
app.Run();
public partial class Program;