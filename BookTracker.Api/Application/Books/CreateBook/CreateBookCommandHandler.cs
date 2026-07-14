using BookTracker.Api.Domain.Books;
using BookTracker.Api.Storage.Books;

namespace BookTracker.Api.Application.Books.CreateBook;

public class CreateBookCommandHandler(IBookRepository bookRepository) : IHandler
{
  public async Task<CreateBookResponse> Execute(CreateBookRequest request)
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
}