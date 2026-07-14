using System.Net;
using BookTracker.Api.Domain.Books;

namespace BookTracker.Api.Tests.IntegrationTests.Books.DeleteBook;

public class DeleteBookTests : IntegrationTest
{
  // private readonly CustomWebApplicationFactory factory = new();
  // |---> DONE BY IntegrationTest

  [Fact]
  public async Task Delete_Book_Deletes_Book()
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
    var deleted = await Client.DeleteAsync("/books/1");
    // Uses HttpResponseAssertions
    await deleted.ShouldHaveStatusCode(HttpStatusCode.NoContent);

    Assert.Equal(HttpStatusCode.NoContent, deleted.StatusCode);

    // var reader = factory.GetReader(); ---> DONE BY IntegrationTest
    var book = Reader.Query(db => db.Books.Find(1));
    Assert.Null(book);
  }

  [Fact]
  public async Task Delete_Book_NotFound_When_DoesNotExist()
  {
    // var client = factory.CreateClient(); ---> DONE BY IntegrationTest
    var response = await Client.DeleteAsync("/books/9999");
    // Uses HttpResponseAssertions
    await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
  }
}