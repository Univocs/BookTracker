using BookTracker.Api.Domain;

namespace BookTracker.Api.Storage;

public class InMemoryBookRepository : IBookRepository
{
  private readonly List<Book> books = [];  // Books are stored in fake memory
  private int nextId = 1;                  // Id counter

  //-----------------------------------------------------------
  public Task<IReadOnlyList<Book>> GetAllAsync()
  {
    return Task.FromResult<IReadOnlyList<Book>>(books);
    // returns from task result, a list books from <Book>
  }

  public Task<Book?> GetByIdAsync(int id)
  {
    var book = books.FirstOrDefault(book => book.Id == id);
    return Task.FromResult(book);
    // returns the book that matches the id in books
  }
  
  public Task<Book> AddAsync(Book book)
  {
    book.Id = nextId;
    nextId++;
    books.Add(book);
    return Task.FromResult(book);
    // nextId adds a new book.Id on the new book into books
  }

  public Task<bool> DeleteAsync(int id)
  {
    var book = books.FirstOrDefault(book => book.Id == id);

    if (book is null) return Task.FromResult(false);
    
    books.Remove(book);
    return Task.FromResult(true);
  }
}