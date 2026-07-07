using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Booklist;
using BookTracker.Api.Domain;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BookTracker.Api.Tests.IntegrationTests.BookList;

public class BookListTests : IntegrationTest
{
  // private readonly CustomWebApplicationFactory factory = new(); 
  // |---> DONE BY IntegrationTest

  [Fact]
  public async Task Get_Books_Returns_Books()
  {
    // var writer = factory.GetWriter(); ---> DONE BY IntegrationTest
    Writer.Seed(db => db.Books.Add(
      new Book
      {
        Title = "Cannery Row",
        Author = "John Steinbeck",
        Year = 1945
      }
    ));

    // var client = factory.CreateClient(); ---> DONE BY IntegrationTest
    var response = await Client.GetAsync("/books");
    // GET request to /books -> HTTP response back

    var allBooks = await response.Content.ReadFromJsonAsync<List<BookInfo>>();
    // JSON response body into a List<BookInfo>

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    Assert.NotNull(allBooks);

    var bookInfo = Assert.Single(allBooks);
    Assert.Equal("Cannery Row", bookInfo.Title);
    Assert.Equal("John Steinbeck", bookInfo.Author);
  }
}