using BookTracker.Api.Wiring;

var builder = WebApplication.CreateBuilder(args); // create builder for WebApplication
builder.AddBookTracker();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseBookTracker();
app.UseCors();
app.Run();
public partial class Program;