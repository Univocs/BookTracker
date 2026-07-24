using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Books.UpdateBook;
using BookTracker.Api.Domain.Books;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.Books.UpdateBook;

public class UpdateBookTests : IntegrationTest
{
  // private readonly CustomWebApplicationFactory factory = new();
  // ---> DONE BY IntegrationTest

  [Fact]
  public async Task Update_Book_Updates_book()
  {
    await AuthenticateAsMember(MemberRole.Administrator);
    // var writer = factory.GetWriter(); ---> DONE BY IntegrationTest
    Writer.Seed(db => db.Books.Add(
      new Book
      {
        Title = new BookTitle("Dune"),
        Author = new AuthorName("Frank Herbert"),
        Year = 1965
      }
    ));

    var request = new UpdateBookRequest
    {
      Title = "Dune Messiah",
      Author = "Frank Herbert",
      Year = 1969
    };

    // var client = factory.CreateClient(); ---> DONE BY IntegrationTest
    var response = await Client.PutAsJsonAsync("/books/1", request);
    
    // Uses HttpResponseAssertions
    await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

    // var reader = factory.GetReader(); ---> DONE BY IntegrationTest
    var book = Reader.Query(db => db.Books.Find(1));
    Assert.NotNull(book);
    Assert.Equal("Dune Messiah", book.Title.Value);
    Assert.Equal("Frank Herbert", book.Author.Value);
    Assert.Equal(1969, book.Year);
  }

  [Fact]
  public async Task Put_Book_NotFound_When_Member_DoesNotExist()
  {
    await AuthenticateAsMember(MemberRole.Administrator);
    
    var request =
        new UpdateBookRequest
        {
          Title = "Unknown Book",
          Author = "Unknown Author",
          Year = 2000
        };

    // var client = factory.CreateClient(); ---> DONE BY IntegrationTest
    var response = await Client.PutAsJsonAsync("/books/9999", request);
    // Uses HttpResponseAssertions
    await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
  }
}