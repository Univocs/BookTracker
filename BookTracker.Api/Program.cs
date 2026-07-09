using BookTracker.Api.Storage;
using Microsoft.EntityFrameworkCore;
using BookTracker.Api.Endpoints;
using BookTracker.Api.Application.BookList;
using BookTracker.Api.Application.GetBookById;
using BookTracker.Api.Application.CreateBook;
using BookTracker.Api.Application.UpdateBook;
using BookTracker.Api.Application.DeleteBook;
using BookTracker.Api.Seeding;

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
builder.Services.AddScoped<GetBookListQuery>();
builder.Services.AddScoped<GetBookByIdQuery>();
builder.Services.AddScoped<CreateBookCommandHandler>();
builder.Services.AddScoped<UpdateBookCommandHandler>();
builder.Services.AddScoped<DeleteBookCommandHandler>();

// We build the app with .Build()
var app = builder.Build();

// This ensures that if the database does not yet exist, it is automatically created.
if (app.Environment.IsDevelopment())
{
  using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        dbContext.Database.EnsureCreated();
        if(builder.Configuration.GetValue<bool>("SeedDatabase"))
            DatabaseSeeder.SeedBooks(dbContext, 500);
    }
}

app.MapBookEndpoints();
// BookEndpoints => Where all the mappings are located for the api

app.Run();

// Important for integrationtests met WebApplicationFactory
public partial class Program;