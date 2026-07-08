using BookTracker.Api.Application.Booklist;
using BookTracker.Api.Storage;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Api.Application.BookList;

public class GetBookListQuery(AppDbContext dbContext)
{
  public async Task<IReadOnlyList<BookInfo>> Execute()
  // A query only has one task = to Execute()

  {
    return await dbContext.Books  // Start with the Books table
        .AsNoTracking() // "Only reading — don't track these for changes." (Normally entities saved for later edits).
        .Select(book => // For every book row, build BookInfo directly with only Id, Title, Author from the database.
                new BookInfo
                {
                  Id = book.Id,
                  Title = book.Title.Value,
                  Author = book.Author.Value 
                  // BookInfo as a response DTO doesn't need to make new objects BookTitle && AuthorName.
                })
                .ToListAsync();  // Run the query against the database and put the results into a List<BookInfo>.
                                 // Send this SQL to the database, do the work and get results back in a list<BookInfo>.
  }
}