using BookTracker.Api.Application;
using BookTracker.Api.Application.CreateBook;
using BookTracker.Api.Storage;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args); // create builder for WebApplication

// DbContext !! = one conversation with the DB, meant to be thrown away when done
// Singleton = ALL requests share the same conversation -> data leaks between users
builder.Services.AddDbContext<AppDbContext>(options =>
{
  options.UseSqlite(builder.Configuration.GetConnectionString("BookTracker"));
});

// Scoped = fresh AppDbContext + EfBookRepository per request
// thrown away after -> no cross-contamination between users
builder.Services.AddScoped<IBookRepository, EfBookRepository>();
builder.Services.AddScoped<BookService>();

// We build the app with .Build()
var app = builder.Build();

// This ensures that if the database does not yet exist, it is automatically created.
if (app.Environment.IsDevelopment())
{
  using (var scope = app.Services.CreateScope())
  {
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
  }
}

app.MapGet("/books", async (BookService service) => Results.Ok(await service.GetAllBooks()));
// Use bookservice to await GetAllBooks()

app.MapPost("/books", async (CreateBookRequest request, BookService service) =>
{
  var response = await service.CreateBook(request);
  return Results.Created($"/books/{response.Id}", response);
});

app.Run();

// Important for integrationtests met WebApplicationFactory
public partial class Program;