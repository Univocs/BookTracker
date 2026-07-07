using BookTracker.Api.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace BookTracker.Api.Tests.IntegrationTests;

// EfWriter is EfReader's counterpart — instead of reading data,
// it lets a test insert ("seed") data into the test database beforehand.
public class EfWriter(IServiceProvider services)
{
    // "seed" is an Action (no return value) provided by the caller:
    // "given a db context, do something with it" (e.g. add a Book)
    public void Seed(Action<AppDbContext> seed)
    {
        // Same pattern as EfReader: create a temporary scope,
        // get a fresh AppDbContext from it.
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Run the caller's action (e.g. db.Books.Add(new Book {...}))
        seed(db);

        // EF Core doesn't write to the database automatically —
        // SaveChanges() is what actually persists the change.
        db.SaveChanges();
    }
}