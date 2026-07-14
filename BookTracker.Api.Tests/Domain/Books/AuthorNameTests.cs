using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Books;

namespace BookTracker.Api.Tests.Domain.Books;

public class AuthorNameTests
{
  [Fact]
  public void AuthorName_Accepts_Author()
  {
    var author  = new AuthorName("F. Scott Fitzgerald");
    Assert.Equal("F. Scott Fitzgerald", author.Value);
  }

  [Fact]
  public void AuthorName_Trims_Value()
  {
    var author  = new AuthorName("  F. Scott Fitzgerald  ");
    Assert.Equal("F. Scott Fitzgerald", author.Value);
  }

  [Fact]
  public void AuthorName_Rejects_Whitespace()
  {
    var exception = Assert.Throws<DomainException>(() => new AuthorName("    "));
    Assert.Equal("Author is required.", exception.Message);
  }

  [Fact]
  public void AuthorName_Rejects_Title_Longer_Than_100Chars()
  {
    var tooLong = new string ('x', 101);
    var exception = Assert.Throws<DomainException>(() => new AuthorName(tooLong));
    Assert.Equal("Author cannot be longer than 100 characters.", exception.Message);
  }
}