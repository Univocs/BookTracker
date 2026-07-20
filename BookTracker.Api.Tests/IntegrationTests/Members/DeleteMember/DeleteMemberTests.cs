using System.Net;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.Members.DeleteMember;

public class DeleteMemberTests : IntegrationTest
{
  [Fact]
  public async Task Delete_Member_Deletes_Member()
  {
    var memberId = await AuthenticateAsMember();

    Writer.Seed(db => db.Members.Add(
      new Member
      {
        Name = new MemberName("Liam Neeson"),
        Email = new MemberEmail("Liam@hotmail.com"),
        PasswordHash = "test-password-hash"
      }
    ));

    var deletedMember = await Client.DeleteAsync($"/members/{memberId}");
    await deletedMember.ShouldHaveStatusCode(HttpStatusCode.NoContent);

    var member = Reader.Query(db => db.Members.Find(1));
    Assert.Null(member);
  }

  [Fact]
  public async Task Delete_Member_NotFound_When_Id_NonExisting()
  {
    var memberId = await AuthenticateAsMember(); // Create member 
    Writer.Seed(db =>                            // Delete member 
    {
        var member = db.Members.Find(memberId);
        if (member is not null) db.Members.Remove(member);
    });

    var nonExistingMember = await Client.DeleteAsync($"/members/{memberId}");
    await nonExistingMember.ShouldHaveStatusCode(HttpStatusCode.NotFound);
  }
}