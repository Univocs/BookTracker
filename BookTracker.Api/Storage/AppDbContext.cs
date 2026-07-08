using BookTracker.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Api.Storage;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
  public DbSet<Book> Books => Set<Book>();

  // this is where you configure things EF can't figure out on its own,
  // like how to store your custom value objects (BookTitle, AuthorName).
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    // Configure mapping rules specifically for the Book entity.
    modelBuilder.Entity<Book>(book =>
    {
      book.Property(b => b.Title)
          .HasConversion(
              title => title.Value,          // saving to DB: value object -> plain string
              value => new BookTitle(value))  // reading from DB: string -> value object
          .HasMaxLength(BookTitle.MaxLength); // matches DB column length to domain rule

      book.Property(b => b.Author)
          .HasConversion(
              author => author.Value,
              value => new AuthorName(value))
          .HasMaxLength(AuthorName.MaxLength);
    });
  }
}

/*AppDbContext is your "connection to the database." 
  Books is the table inside it. This whole class replaces the 
  InMemoryBookRepository's List<Book> — instead of a list in RAM, 
  Books now points at a real database table, but you interact with it 
  using nearly identical LINQ syntax.*/