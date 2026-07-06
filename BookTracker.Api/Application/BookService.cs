using BookTracker.Api.Application.Booklist;
using BookTracker.Api.Storage;

namespace BookTracker.Api.Application;

public class BookService(IBookRepository bookRepository)
{
  public async Task<IReadOnlyList<BookInfo>> GetAllBooks()
  {
    var books = await bookRepository.GetAllAsync();
    var summary = books.Select(b => new BookInfo{
      Title = b.Title, 
      Author = b.Author
      });
    return summary.ToList();
  }
}