using BookTracker.Api.Application;
using BookTracker.Api.Storage;

var builder = WebApplication.CreateBuilder(args); // create builder for WebApplication

// Singleton: one shared instance for the whole app's lifetime.
// Needed here so the in-memory book list isn't wiped every request.
builder.Services.AddSingleton<IBookRepository, InMemoryBookRepository>();

// Scoped: one instance per HTTP request.
builder.Services.AddScoped<BookService>();

// We build the app with .Build()
var app = builder.Build();

app.MapGet("/books", async (BookService service) => Results.Ok(await service.GetAllBooks()));
// Use bookservice to await GetAllBooks()

app.Run();

// Important for integrationtests met WebApplicationFactory
public partial class Program;