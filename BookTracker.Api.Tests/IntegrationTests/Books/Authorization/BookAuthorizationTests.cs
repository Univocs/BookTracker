using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Books.CreateBook;
using BookTracker.Api.Application.Books.UpdateBook;
using BookTracker.Api.Domain.Books;

namespace BookTracker.Api.Tests.IntegrationTests.Books.Authorization;

public class BookAuthorizationTests : IntegrationTest
{
  [Fact]
  public async Task Create_Book_Requires_Authentication()
  {
    var request = new CreateBookRequest
    {
      Title = "Dune",
      Author = "Frank Herbert",
      Year = 1965
    }; // Not authenticated as a member!

    var response = await Client.PostAsJsonAsync("/books", request);
    await response.ShouldHaveStatusCode(HttpStatusCode.Unauthorized);

    var count = Reader.Query(db => db.Books.Count());
    Assert.Equal(0, count); // Did it save any books?
  }

//-------------------------------------------------------------------

  [Fact]
  public async Task Update_Book_Requires_Authentication()
  {
    Writer.Seed(db => db.Books.Add(new Book
    {
      Title = new BookTitle("Dune"),
      Author = new AuthorName("Frank Herbert"),
      Year = 1965
    }));

    var updateRequest = new UpdateBookRequest
    {
      Title = "Dune Messiah",
      Author = "Frank Herbert",
      Year = 1969
    };

    var response = await Client.PutAsJsonAsync("/books/1", updateRequest);
    await response.ShouldHaveStatusCode(HttpStatusCode.Unauthorized);
  }

//-------------------------------------------------------------------

  [Fact]
  public async Task Delete_Book_Requires_Authentication()
  {
    Writer.Seed(db => db.Books.Add(new Book
    {
      Title = new BookTitle("Dune"),
      Author = new AuthorName("Frank Herbert"),
      Year = 1965
    }));

    var deleted = await Client.DeleteAsync("/books/1");
    await deleted.ShouldHaveStatusCode(HttpStatusCode.Unauthorized);
  }

//-------------------------------------------------------------------

  [Fact]
  public async Task Get_Books_Does_Not_Require_Authentication()
  {
    var response = await Client.GetAsync("/books");
    await response.ShouldHaveStatusCode(HttpStatusCode.OK);
  }

//-------------------------------------------------------------------

  [Fact]
  public async Task Get_BookById_Does_Not_Require_Authentication()
  {
    Writer.Seed(db => db.Books.Add(new Book
    {
      Title = new BookTitle("Dune"),
      Author = new AuthorName("Frank Herbert"),
      Year = 1965
    }));

    var response = await Client.GetAsync("/books/1");
    await response.ShouldHaveStatusCode(HttpStatusCode.OK);
  }
}