using BookTracker.Api.Domain;

namespace BookTracker.Api.Storage;

public interface IBookRepository
{
  Task <Book> AddAsync(Book book);
  // Adds a book -- returned with updated Id.
  Task <bool> UpdateAsync(Book book);
  Task <bool> DeleteAsync(int id);
  // Returns true || false -- deleted or not.
}