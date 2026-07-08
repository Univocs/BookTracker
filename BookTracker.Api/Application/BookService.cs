using BookTracker.Api.Application.Booklist;
using BookTracker.Api.Application.CreateBook;
using BookTracker.Api.Application.GetBookById;
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
      Title = b.Title.Value,
      Author = b.Author.Value
      // BookInfo as a response DTO doesn't need to make new objects
      // So no "new BookTitle()" or "new AuthorName(), only for creation!
    });
    return summary.ToList();
  }

  public async Task<BookDetails?> GetBookById(int id)
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

  public async Task<CreateBookResponse> CreateBook(CreateBookRequest request)
  {
    var book = new Book  // We make a new book
    {
      Title = new BookTitle(request.Title),
      Author = new AuthorName(request.Author),
      Year = request.Year
    };

    var savedBook = await bookRepository.AddAsync(book); // Save book to bookRepository

    return new CreateBookResponse
    {
      Id = savedBook.Id,
      Title = savedBook.Title.Value,
      Author = savedBook.Author.Value,
      Year = savedBook.Year
    };
  }

  public async Task<bool> UpdateBook(int id, UpdateBookRequest request)
  {
    var book = new Book
    {
      Id = id,
      Title = new BookTitle(request.Title),
      Author = new AuthorName(request.Author),
      Year = request.Year
    };

    return await bookRepository.UpdateAsync(book);
  }

  public async Task<bool> DeleteBook(int id)
  {
    return await bookRepository.DeleteAsync(id);
  }
}