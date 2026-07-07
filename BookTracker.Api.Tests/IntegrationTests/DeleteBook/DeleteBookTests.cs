using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.CreateBook;
using BookTracker.Api.Domain;

namespace BookTracker.Api.Tests.IntegrationTests.DeleteBook;

public class DeleteBookTests
{
  private readonly CustomWebApplicationFactory factory = new();

  [Fact]
  public async Task Delete_Book_Deletes_Book()
  {
    var writer = factory.GetWriter();
    writer.Seed(db => db.Books.Add(
      new Book
      {
        Title = "Dune",
        Author = "Frank Herbert",
        Year = 1965
      }
    ));

    var client = factory.CreateClient();
    var deleted = await client.DeleteAsync("/books/1");
    Assert.Equal(HttpStatusCode.NoContent, deleted.StatusCode);

    var reader = factory.GetReader();
    var book = reader.Query(db => db.Books.Find(1));
    Assert.Null(book);
  }

  [Fact]
  public async Task Delete_Book_Not_Found_DoesNotExist()
  {
    var client = factory.CreateClient();
    var response = await client.DeleteAsync("/books/9999");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
  }
}