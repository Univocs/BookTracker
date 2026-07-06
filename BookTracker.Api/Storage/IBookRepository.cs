using BookTracker.Api.Domain;

namespace BookTracker.Api.Storage;

public interface IBookRepository
{
  Task <IReadOnlyList<Book>> GetAllAsync();
  // Returns task -- produces readonly list of books.
  Task <Book?> GetByIdAsync (int Id);
  // Returns task -- book by Id -- return null possible.
  Task <Book> AddAsync(Book book);
  // Adds a book -- returned with updated Id.
  Task <bool> DeleteAsync(int id);
  // Returns true || false -- deleted or not.
}