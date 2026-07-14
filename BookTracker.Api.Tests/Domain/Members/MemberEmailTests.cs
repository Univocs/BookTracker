using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.Domain.Members;

public class MemberEmailTests
{
  [Fact]
  public void MemberEmail_Accepts_Valid_Email()
  {
    var email = new MemberEmail("philmil@gmail.com");
    Assert.Equal("philmil@gmail.com", email.Value);
  }

  [Fact]
  public void MemberEmail_Trims_The_Value()
  {
    var email = new MemberEmail("   philmil@gmail.com   ");
    Assert.Equal("philmil@gmail.com", email.Value);
  }

  [Fact]
  public void MemberEmail_Rejects_Whitespace()
  {
    var exception = Assert.Throws<DomainException>(() => new MemberEmail("    "));
    Assert.Equal("Member email is required.", exception.Message);
  }

  [Fact]
  public void MemberEmail_Rejects_Email_Longer_Than_200Chars()
  {
    var EmailTooLong = new string ('@', 201);
    var exception = Assert.Throws<DomainException>(() => new MemberEmail(EmailTooLong));
    Assert.Equal("Member email cannot be longer than 200 characters.", exception.Message);
  }

  [Fact]
  public void MemberEmail_Rejects_Email_Without_At()
  {
    var exception = Assert.Throws<DomainException>(() => new MemberEmail("philmil_gmail.com"));
    Assert.Equal("Member email needs to contain '@'.", exception.Message);
  }
}