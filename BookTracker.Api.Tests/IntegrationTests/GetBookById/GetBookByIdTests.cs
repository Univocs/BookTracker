using System.Net;
using BookTracker.Api.Application.GetBookById;
using BookTracker.Api.Domain;

namespace BookTracker.Api.Tests.IntegrationTests.GetBookById;

public class GetBookByIdTests : IntegrationTest
{
  // private readonly CustomWebApplicationFactory factory = new();
  // ---> DONE BY IntegrationTest

  [Fact]
  public async Task GetBookById_Returns_Book()
  {
    // var writer = factory.GetWriter(); ---> DONE BY IntegrationTest
    Writer.Seed(db => db.Books.Add(
      new Book
      {
        Title = new BookTitle("Dune"),
        Author = new AuthorName("Frank Herbert"),
        Year = 1965
      }
    ));

    // var client = factory.CreateClient(); ---> DONE BY IntegrationTest

    var response = await Client.GetAsync("/books/1");
    var book = await response.ReadJsonAs<BookDetails>(HttpStatusCode.OK);
    // JSON response body into <BookDetails> only if HttpStatusCode is OK.

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    Assert.NotNull(book);
    Assert.Equal(1, book.Id);
    Assert.Equal("Dune", book.Title);
    Assert.Equal("Frank Herbert", book.Author);
    Assert.Equal(1965, book.Year);
  }

  [Fact]
  public async Task GetBookById_Returns_NotFound_When_Book_Does_NotExist()
  {
    // var client = factory.CreateClient(); ---> DONE BY IntegrationTest
    var response = await Client.GetAsync("/books/22220");
    await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
    
    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
  }
}