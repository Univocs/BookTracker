using BookTracker.Api.Domain.Actors;
using BookTracker.Api.Domain.Books;
using BookTracker.Api.Storage.Books;

namespace BookTracker.Api.Application.Books.UpdateBook;

public class UpdateBookCommandHandler(IBookRepository bookRepository) : IHandler
{
  public async Task<bool> Execute(Actor actor, 
                                  int id,
                                  UpdateBookRequest request)
  {
    BookPermissions.EnsureCanManage(actor);
    var book = new Book
    {
      Id = id,
      Title = new BookTitle(request.Title),
      Author = new AuthorName(request.Author),
      Year = request.Year
    };

    return await bookRepository.UpdateAsync(book);
  }
}