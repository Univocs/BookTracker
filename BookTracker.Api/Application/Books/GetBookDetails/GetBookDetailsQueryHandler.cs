using BookTracker.Api.Storage;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Api.Application.Books.GetBookDetails;

public class GetBookDetailsQueryHandler(AppDbContext dbContext) : IHandler
{
  public async Task<GetBookDetailsResponse?> Execute(int id)
  // A query only has one task = to Execute()

  {
    return await dbContext.Books // Start with the Books table
    .AsNoTracking() // "Only reading — don't track these for changes." (Normally entities saved for later edits).
    .Where(book => book.Id == id) // execute when the book id in books == int id
    .Select(book => // For every book row, build BookDetails directly with Id, Title, Author and Year.
    new GetBookDetailsResponse
    {
      Id = book.Id,
      Title = book.Title.Value,
      Author = book.Author.Value,
      Year = book.Year
    })
    .FirstOrDefaultAsync();
    // If there's no book with this id, return query null
  }
}