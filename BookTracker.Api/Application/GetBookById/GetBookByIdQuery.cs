using BookTracker.Api.Storage;

namespace BookTracker.Api.Application.GetBookById;

public class GetBookByIdQuery(IBookRepository bookRepository)
{
  public async Task<BookDetails?> Execute(int id)
  // A query only has one task = to Execute()

  {
    var book = await bookRepository.GetByIdAsync(id);
    if (book is null) return null;

    return
        new BookDetails
        {
          Id = book.Id,
          Title = book.Title.Value,
          Author = book.Author.Value,
          Year = book.Year
        };
  }
}