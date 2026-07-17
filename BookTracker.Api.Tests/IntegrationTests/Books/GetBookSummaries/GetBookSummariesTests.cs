using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application;
using BookTracker.Api.Application.Books.GetBookSummaries;
using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Books;

namespace BookTracker.Api.Tests.IntegrationTests.Books.GetBookSummaries;

public class BookListTests : IntegrationTest
{
  // private readonly CustomWebApplicationFactory factory = new(); 
  // |---> DONE BY IntegrationTest

  [Fact]
  public async Task Get_Book_Summaries_Returns_Book_Summaries()
  {
    // var writer = factory.GetWriter(); ---> DONE BY IntegrationTest
    Writer.Seed(db => db.Books.Add(
      new Book
      {
        Title = new BookTitle("Cannery Row"),
        Author = new AuthorName("John Steinbeck"),
        Year = 1945
      }
    ));

    // var client = factory.CreateClient(); ---> DONE BY IntegrationTest
    var response = await Client.GetAsync("/books");
    // GET request to /books -> HTTP response back

    // Uses HttpResponseAssertions
    var result = await response.ReadJsonAs<PagedResult<BookSummary>>(HttpStatusCode.OK);
    // JSON response body into a PagedResult<BookInfo> only if HttpStatusCode is OK.
    Assert.NotNull(result);

    var bookInfo = Assert.Single(result.Items);

    Assert.Equal("Cannery Row", bookInfo.Title);
    Assert.Equal("John Steinbeck", bookInfo.Author);
    Assert.Equal(1, result.Page);
    Assert.Equal(10, result.PageSize);
    Assert.Equal(1, result.TotalItems);
    Assert.Equal(1, result.TotalPages);
  }

  [Fact]
  public async Task Get_Book_Summaries_Returns_RequestedPage()
  {
    Writer.Seed(db =>
    {
      db.Books.AddRange(  // AddRange adds multiple entities at once
          new Book
          {
            Title = new BookTitle("Book 1"),
            Author = new AuthorName("Author 1"),
            Year = 2001
          },
          new Book
          {
            Title = new BookTitle("Book 2"),
            Author = new AuthorName("Author 2"),
            Year = 2002
          },
          new Book
          {
            Title = new BookTitle("Book 3"),
            Author = new AuthorName("Author 3"),
            Year = 2003
          });
    });

    var result = await Client.GetFromJsonAsync<PagedResult<BookSummary>>("/books?page=2&pageSize=1");

    Assert.NotNull(result);

    var book = Assert.Single(result.Items);

    Assert.Equal("Book 2", book.Title);   // confirms the SECOND book, Skip/Take landed on the right one
    Assert.Equal(2, result.Page);         // response echoes back "you asked for page 2"
    Assert.Equal(1, result.PageSize);     // and "pageSize was 1"
    Assert.Equal(3, result.TotalItems);   // 3 books exist in total, regardless of paging
    Assert.Equal(3, result.TotalPages);   // 3 books / 1 per page = 3 total pages
  }

  [Fact]
  public async Task Get_Books_Summaries_Returns_EmptyItems_When_Page_TooHigh()
  {
    Writer.Seed(db =>
    {
      db.Books.Add(
          new Book
          {
            Title = new BookTitle("Book 1"),
            Author = new AuthorName("Author 1"),
            Year = 2001
          });
    });

    var result = await Client.GetFromJsonAsync<PagedResult<BookSummary>>("/books?page=99&pageSize=10");

    Assert.NotNull(result);              // API still responded successfully — no 404, no error
    Assert.Empty(result.Items);          // no books came back — makes sense, page 99 has none
    Assert.Equal(99, result.Page);       // the response still reports back "you asked for page 99"
    Assert.Equal(10, result.PageSize);   // and "here's the pageSize you used"
    Assert.Equal(1, result.TotalItems);  // there's still 1 book in total, in the whole database
    Assert.Equal(1, result.TotalPages);  // and with 1 book / pageSize 10, only 1 page actually exists
  }

  [Fact]
  public async Task Get_Books_Summaries_Can_Search_ByTitle()
  {
    Writer.Seed(db =>
    {
      db.Books.AddRange(
          new Book
          {
            Title = new BookTitle("Dune"),
            Author = new AuthorName("Frank Herbert"),
            Year = 1965
          },
          new Book
          {
            Title = new BookTitle("The Big Sleep"),
            Author = new AuthorName("Raymond Chandler"),
            Year = 1939
          });
    });

    var response = await Client.GetAsync("/books?search=dune");

    var result = await response.ReadJsonAs<PagedResult<BookSummary>>(HttpStatusCode.OK);

    var book = Assert.Single(result.Items);

    Assert.Equal("Dune", book.Title);
    Assert.Equal("Frank Herbert", book.Author);
    Assert.Equal(1, result.TotalItems);
    Assert.Equal(1, result.TotalPages);
  }

  [Fact]
  public async Task Get_Books_Summaries_Can_Search_ByAuthor()
  {
    Writer.Seed(db =>
    {
      db.Books.AddRange(
          new Book
          {
            Title = new BookTitle("Dune"),
            Author = new AuthorName("Frank Herbert"),
            Year = 1965
          },
          new Book
          {
            Title = new BookTitle("The Big Sleep"),
            Author = new AuthorName("Raymond Chandler"),
            Year = 1939
          });
    });

    var response = await Client.GetAsync("/books?search=Frank");

    var result = await response.ReadJsonAs<PagedResult<BookSummary>>(HttpStatusCode.OK);

    var book = Assert.Single(result.Items);

    Assert.Equal("Dune", book.Title);
    Assert.Equal("Frank Herbert", book.Author);
    Assert.Equal(1, result.TotalItems);
    Assert.Equal(1, result.TotalPages);
  }


  [Fact]
  public async Task Get_Books_Summaries_Applies_Paging_After_Search()
  {
    Writer.Seed(db =>
    {
      db.Books.AddRange(
          new Book
          {
            Title = new BookTitle("Dune"),
            Author = new AuthorName("Frank Herbert"),
            Year = 1965
          },
          new Book
          {
            Title = new BookTitle("Dune Messiah"),
            Author = new AuthorName("Frank Herbert"),
            Year = 1969
          },
          new Book
          {
            Title = new BookTitle("The Big Sleep"),
            Author = new AuthorName("Raymond Chandler"),
            Year = 1939
          });
    });

    var response = await Client.GetAsync("/books?search=dune&page=2&pageSize=1");

    var result = await response.ReadJsonAs<PagedResult<BookSummary>>(HttpStatusCode.OK);

    var book = Assert.Single(result.Items);

    Assert.Equal("Dune Messiah", book.Title);
    Assert.Equal(2, result.Page);
    Assert.Equal(1, result.PageSize);
    Assert.Equal(2, result.TotalItems);
    Assert.Equal(2, result.TotalPages);
  }

  [Fact]
  public async Task Get_Books_Summaries_Returns_Empty_List_When_Search_NoResults()
  {
    Writer.Seed(db =>
    {
      db.Books.AddRange(
          new Book
          {
            Title = new BookTitle("Dune"),
            Author = new AuthorName("Frank Herbert"),
            Year = 1965
          });
    });

    var response = await Client.GetAsync("/books?search=Raymond");
    var result = await response.ReadJsonAs<PagedResult<BookSummary>>(HttpStatusCode.OK);

    Assert.NotNull(result);
    Assert.Empty(result.Items);
  }

[Fact]
    public void BookTitle_And_BookAuthor_RejectsNull()
    {
        var titleException = Assert.Throws<DomainException>(() => new BookTitle(null!));
        var authorException = Assert.Throws<DomainException>(() => new AuthorName(null!));

        Assert.Equal("Title is required.", titleException.Message);
        Assert.Equal("Author is required.", authorException.Message);
    }
}