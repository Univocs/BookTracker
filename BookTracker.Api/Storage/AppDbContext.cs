using BookTracker.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Api.Storage;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
  public DbSet<Book> Books => Set<Book>();
}

/*AppDbContext is your "connection to the database." 
  Books is the table inside it. This whole class replaces the 
  InMemoryBookRepository's List<Book> — instead of a list in RAM, 
  Books now points at a real database table, but you interact with it 
  using nearly identical LINQ syntax.*/