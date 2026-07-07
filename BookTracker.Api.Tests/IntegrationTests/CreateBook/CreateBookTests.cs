using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.CreateBook;
using BookTracker.Api.Domain;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BookTracker.Api.Tests.IntegrationTests.CreateBook;

public class CreateBookTests
{
  private readonly CustomWebApplicationFactory factory = new();

  [Fact]
  public async Task Post_Book_Creates_Book()
  {
    var request = new CreateBookRequest
    {
      Title = "The Heart Is a Lonely Hunter",
      Author = "Carson McCullers",
      Year = 1940
    };

    var client = factory.CreateClient();
    var response = await client.PostAsJsonAsync("/books", request);
    var created = await response.Content.ReadFromJsonAsync<CreateBookResponse>();
    Assert.NotNull(created);
    
    var reader = factory.GetReader();
    var book = reader.Query(context => context.Find<Book>(created.Id));

    
    Assert.NotNull(book);
    Assert.Equal("The Heart Is a Lonely Hunter", book.Title);
    Assert.Equal("Carson McCullers", book.Author);
    Assert.Equal(1940, book.Year);
  }
}