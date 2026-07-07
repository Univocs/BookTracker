using BookTracker.Api.Application.Booklist;
using BookTracker.Api.Application.CreateBook;
using BookTracker.Api.Application.UpdateBook;
using BookTracker.Api.Domain;
using BookTracker.Api.Storage;

namespace BookTracker.Api.Application;

public class BookService(IBookRepository bookRepository)
{
  public async Task<IReadOnlyList<BookInfo>> GetAllBooks()
  {
    var books = await bookRepository.GetAllAsync();
    var summary = books.Select(b => new BookInfo
    {
      Title = b.Title,
      Author = b.Author
    });
    return summary.ToList();
  }

  public async Task<CreateBookResponse> CreateBook(CreateBookRequest request)
  {
    var book = new Book  // We make a new book
    {
      Title = request.Title,
      Author = request.Author,
      Year = request.Year
    };

    var savedBook = await bookRepository.AddAsync(book); // Save book to bookRepository

    return new CreateBookResponse
    {
      Id = savedBook.Id,
      Title = savedBook.Title,
      Author = savedBook.Author,
      Year = savedBook.Year
    };
  }

  public async Task<bool> UpdateBook(int id, UpdateBookRequest response)
  {
    var book = new Book
    {
      Id = id,
      Title = response.Title,
      Author = response.Author,
      Year = response.Year
    };

    return await bookRepository.UpdateAsync(book);
  }

  public async Task<bool> DeleteBook(int id)
  {
    return await bookRepository.DeleteAsync(id);
  }
}