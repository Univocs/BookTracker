using BookTracker.Api.Domain.Actors;
using BookTracker.Api.Domain.Books;
using BookTracker.Api.Storage.Books;

namespace BookTracker.Api.Application.Books.DeleteBook;

public class DeleteBookCommandHandler(IBookRepository bookRepository) : IHandler
{
  public async Task<bool> Execute(Actor actor, int id)
  {
    BookPermissions.EnsureCanManage(actor);
    return await bookRepository.DeleteAsync(id);
  }
}