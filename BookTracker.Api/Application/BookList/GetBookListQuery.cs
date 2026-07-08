using BookTracker.Api.Application.Booklist;
using BookTracker.Api.Storage;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Api.Application.BookList;

/*-----------------PAGING---------------------
------------Filtering and sorting-------------
------------Applying pagination---------------
------------Projecting to BookInfo------------
------------Retrieving the result-------------
*/
public class GetBookListQuery(AppDbContext dbContext)
{
  private const int DefaultPages = 1;
  private const int DefaultPageSize = 10;
  private const int MinPageSize = 1;
  private const int MaxPageSize = 50; // Const for fixed values and private because nothing outside needs them

  // Before: used to return Task<IReadOnlyList<BookInfo>> (a plain list). 
  // Now it returns Task<PagedResult<BookInfo>> — the wrapper with metadata. 
  // It also now takes in a parameter, request, which carries whatever page/pageSize the caller asked for.
  public async Task<PagedResult<BookInfo>> Execute(GetBookListRequest request)
  // A query only has one task = to Execute()
  {
    var page = Math.Max(1, request.Page ?? DefaultPages); // 1 is the lowest valid page number.
    // ?? is the null-coalescing operator. "evaluate the left side If not null, use it. If it IS null, use DefaultPages."

    var pageSize = Math.Clamp(request.PageSize ?? DefaultPageSize, MinPageSize, MaxPageSize);
    // Pagesize (if null == DefaultPageSize) is clamped in between MinPageSize && MaxPageSize

    var totalItems = await dbContext.Books.CountAsync();
    // how many rows are in the Books table in total? --> 42 books in db means totalItems = 42

    var books = await dbContext.Books  // Start with the Books table
        .AsNoTracking() // "Only reading — don't track these for changes." (Normally entities saved for later edits).
        .OrderBy(book => book.Id) // Sort all 25 books by Id, happens before Skip/Take because order is needed.
        .Skip((page - 1) * pageSize) // (page 1-1)*10 = 0 → skips nothing, starts from book 1 // (2-1)*10 = 10 → skip first 10, start from 11
        .Take(pageSize) // .Take(10). After what's left (11–25), grab only the next 10. So books 11–20 → page 2!
        .Select(book => // For every book row, build BookInfo directly with only Id, Title, Author from the database.
                new BookInfo
                {
                  Id = book.Id,
                  Title = book.Title.Value,
                  Author = book.Author.Value
                  // BookInfo as a response DTO doesn't need to make new objects BookTitle && AuthorName.
                })
                .ToListAsync();  // Run the query against the database and put the results into a List<BookInfo>.
                                 // Send this SQL to the database, do the work and get results back in a list<BookInfo>.

    return new PagedResult<BookInfo>
    {
      Items = books,
      Page = page,
      PageSize = pageSize,
      TotalItems = totalItems,
      TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
      // 42 / 10.0 (dec) = 4.2 → Math.Ceiling always rounds up to the next whole number.
    };
  }
}