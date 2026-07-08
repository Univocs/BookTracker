using BookTracker.Api.Application.Booklist;
using BookTracker.Api.Storage;

namespace BookTracker.Api.Application.BookList;

public class GetBookListQuery(IBookRepository bookRepository)
{
  public async Task<IReadOnlyList<BookInfo>> Execute()
  // A query only has one task = to Execute()

  {
    var books = await bookRepository.GetAllAsync();
    var summary = books.Select(b => new BookInfo
    {
      Title = b.Title.Value,
      Author = b.Author.Value
      // BookInfo as a response DTO doesn't need to make new objects
      // So no "new BookTitle()" or "new AuthorName(), only for creation!
    });
    return summary.ToList();
  }
}