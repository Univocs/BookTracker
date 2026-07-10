using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.CreateBook;
using BookTracker.Api.Domain;

namespace BookTracker.Api.Tests.IntegrationTests.CreateBook;

public class CreateBookTests : IntegrationTest
{
  // private readonly CustomWebApplicationFactory factory = new();
  // |---> DONE BY IntegrationTest

  [Fact]
  public async Task Post_Book_Creates_Book()
  {
    var request = new CreateBookRequest
    {
      Title = "The Heart Is a Lonely Hunter",
      Author = "Carson McCullers",
      Year = 1940
    };

    // var client = factory.CreateClient(); ---> DONE BY IntegrationTest
    var response = await Client.PostAsJsonAsync("/books", request);
    var created = await response.ReadJsonAs<CreateBookResponse>(HttpStatusCode.Created);
    // status "Created" is checked before Json is being read into var "created"

    Assert.NotNull(created);

    // var reader = factory.GetReader(); ---> DONE BY IntegrationTest
    var book = Reader.Query(context => context.Find<Book>(created.Id));


    Assert.NotNull(book);
    Assert.Equal("The Heart Is a Lonely Hunter", book.Title.Value);
    Assert.Equal("Carson McCullers", book.Author.Value);
    Assert.Equal(1940, book.Year);
  }

  [Fact]
  public async Task PostBookReturnsBadRequestWhenTitleIsWhitespace()
  {
    var request =
        new CreateBookRequest
        {
          Title = "   ",
          Author = "      ",
          Year = 1940
        };

    var response = await Client.PostAsJsonAsync("/books", request);
    await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
    
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
  }
}