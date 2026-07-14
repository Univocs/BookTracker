using BookTracker.Api.Domain.Books;
using BookTracker.Api.Domain;

namespace BookTracker.Api.Tests.Domain.Books;

public class BookTitleTests
{
  [Fact]
  public void BookTitle_Accepts_Title()
  {
    var title = new BookTitle("Duuune");
    Assert.Equal("Duuune", title.Value);
  }

  [Fact]
  public void BookTitle_Trims_Value()
  {
    var title = new BookTitle("  Dune  ");
    Assert.Equal("Dune", title.Value);
  }

  [Fact]
  public void BookTitle_Rejects_Whitespace()
  {
    var exception = Assert.Throws<DomainException>(() => new BookTitle("    "));
    Assert.Equal("Title is required.", exception.Message);
  }

  [Fact]
  public void BookTitle_Rejects_Title_Longer_Than_100Chars()
  {
    var tooLong = new string ('x', 101);
    var exception = Assert.Throws<DomainException>(() => new BookTitle(tooLong));
    Assert.Equal("Title cannot be longer than 100 characters.", exception.Message);
  }
}