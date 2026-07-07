using BookTracker.Api.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace BookTracker.Api.Tests.IntegrationTests;

public class EfReader(IServiceProvider services)
// Primary constructor: this stores "services" (the app's root DI container)
// Every method below can use it.
{
  public T Query<T>(Func<AppDbContext, T> query)
  // Give me a function that takes "AppDbContext" and returns T: can be anything (int, Book, etc.)
  // Example --> reader.Query(db => db.Books.Single(...));    // T = Book
  // EfReader supplies the context itself.
  {
    using var scope = services.CreateScope();
    // AppDbContext is registered as "Scoped" — you can't grab it straight
    // from the root "services". You first need a scope (a temporary,
    // short-lived container) to resolve it safely.

    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // Now ask that scope for a fresh AppDbContext instance.

    return query(db);
    // Run the caller's query against this fresh context, and return the result.
    // "using" above means: once we exit this method, the scope gets auto disposed.
  }
}

/*
  1) AppDbContext is registered as Scoped. That's a DI lifetime rule that says: 
     "one instance per scope, and it must be created and destroyed within a scope."
  2) services (the field in EfReader) is the root provider — 
     it lives for the entire test run, it's not a scope itself.
  3) ASP.NET Core actively blocks you from resolving a Scoped service from the root provider. 
     If you tried services.GetRequiredService<AppDbContext>() directly, it would throw an 
     exception — not just "be wrong," but literally fail at runtime, on purpose, as a safety check.
  4) So the CreateScope() line is the only legal way to get AppDbContext at all when all you have is 
     the root provider. You must create a scope first, then ask that scope for the instance.
  5) Why get a new one every call, instead of creating one scope for the whole EfReader and reusing it? 
     Because you specifically want a clean, isolated AppDbContext for each query — mirroring how the 
     real app also gets a fresh one per HTTP request. It also avoids any weirdness like stale cached 
     data or leftover tracked entities from a previous query bleeding into the next one.
*/