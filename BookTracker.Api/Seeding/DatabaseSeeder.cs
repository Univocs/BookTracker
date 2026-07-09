using BookTracker.Api.Storage;

namespace BookTracker.Api.Seeding;

// The code that actually puts the fake books into the database, only when it makes sense
public static class DatabaseSeeder
{
  //count = 50 is a default parameter value, if no SeedBooks(dbContext) without count, defaults 50. 
  // You can still override it: SeedBooks(dbContext, 200).
  public static void SeedBooks(AppDbContext dbContext, int count = 50)
  {
    // Any() checks: "does the Books table already have at least one row?" 
    // If yes, bail out immediately — do nothing. This is a guard against duplicate seeding
    // you don't want to re-run and keep adding 50 books every time the app starts. It seeds an empty database.
    if (dbContext.Books.Any())
    {
      return;
    }

    // Generate count books, using the fuzzer.
    var books = BookFuzzr.Many(count);

    // Same pattern as EfWriter.Seed, queue all the generated books with AddRange, 
    // then commit them to the database in one go with SaveChanges().
    dbContext.Books.AddRange(books);
    dbContext.SaveChanges();
  }
}   