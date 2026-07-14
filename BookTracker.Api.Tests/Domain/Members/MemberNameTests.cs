using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.Domain.Members;

public class MemberNameTests
{
  [Fact]
  public void MemberName_Accepts_Valid_Name()
  {
    var name = new MemberName("Philip");
    Assert.Equal("Philip", name.Value);
  }

  [Fact]
  public void MemberName_Trims_The_Value()
  {
    var name = new MemberName("   Philip   ");
    Assert.Equal("Philip", name.Value);
  }

  [Fact]
  public void MemberName_Rejects_Whitespace()
  {
    var exception = Assert.Throws<DomainException>(() => new MemberName("    "));
    Assert.Equal("Member name is required.", exception.Message);
  }

  [Fact]
  public void MemberName_Rejects_Name_Longer_Than_100Chars()
  {
    var NameTooLong = new string ('e', 101);
    var exception = Assert.Throws<DomainException>(() => new MemberName(NameTooLong));
    Assert.Equal("Member name cannot be longer than 100 characters.", exception.Message);
  }
}