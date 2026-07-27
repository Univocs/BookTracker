using BookTracker.Api.Domain.Books;

namespace BookTracker.Api.Storage.Books;

public interface IBookRepository
{
  Task <Book> AddAsync(Book book);
  // Adds a book -- returned with updated Id.
  Task <UpdateBookResult> UpdateAsync(Book book, Guid expectedVersion);
  // Book contains the new data -- while expectedVersion is what user read
  Task <bool> DeleteAsync(int id);
  // Returns true || false -- deleted or not.
}