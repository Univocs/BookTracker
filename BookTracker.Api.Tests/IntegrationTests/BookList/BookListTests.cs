using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Booklist;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BookTracker.Api.Tests.IntegrationTests.BookList;

public class BookListTests
{
  private readonly CustomWebApplicationFactory factory = new();

  [Fact]
  public async Task Get_Books_Returns_Books()
  {
    var client = factory.CreateClient();
    var response = await client.GetAsync("/books");
    // GET request to /books -> HTTP response back
    var allBooks = await response.Content.ReadFromJsonAsync<List<BookInfo>>();
    // JSON response body into a List<BookInfo>

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    Assert.NotNull(allBooks);
    Assert.Empty(allBooks);
  }
}