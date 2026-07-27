using BookTracker.Api.Domain.Books;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Api.Storage.Books;

public class EfBookRepository(AppDbContext dbContext) : IBookRepository
{
  public async Task<Book> AddAsync(Book book)
  {
    dbContext.Books.Add(book);
    await dbContext.SaveChangesAsync();
    return book;
  }

  public async Task<UpdateBookResult> UpdateAsync(Book book,
                                                  Guid expectedVersion)
  {
    var existingBook = await dbContext.Books.FindAsync(book.Id);

    if (existingBook is null) return UpdateBookResult.NotFound;

    dbContext.Entry(existingBook) // Access to entity from database
             .Property(current => current.Version) // Get Version Property
             .OriginalValue = expectedVersion; 

    // apply incoming changes 
    existingBook.Title = book.Title;
    existingBook.Author = book.Author;
    existingBook.Year = book.Year;

    existingBook.Version = Guid.NewGuid(); // After update, new version! 

    try // Save updates if no conflict! 
    {
      await dbContext.SaveChangesAsync(); // Saving actually saves the update
      return UpdateBookResult.Updated;
    }
    catch (DbUpdateConcurrencyException) // EF Core exception when version invalid
    {
      return UpdateBookResult.Conflict;
    }
  }

  public async Task<bool> DeleteAsync(int id)
  {
    var book = await dbContext.Books.FindAsync(id);
    if (book is null) return false;


    dbContext.Books.Remove(book);
    await dbContext.SaveChangesAsync();
    return true;
  }
}