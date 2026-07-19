using System.Net;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.Members.DeleteMember;

public class DeleteMemberTests : IntegrationTest
{
  [Fact]
  public async Task Delete_Member_Deletes_Member()
  {
    Writer.Seed(db => db.Members.Add(
      new Member
      {
        Name = new MemberName("Liam Neeson"),
        Email = new MemberEmail("Liam@hotmail.com"),
        PasswordHash = "test-password-hash"
      }
    ));

    var deletedMember = await Client.DeleteAsync("/members/1");
    await deletedMember.ShouldHaveStatusCode(HttpStatusCode.NoContent);

    var member = Reader.Query(db => db.Members.Find(1));
    Assert.Null(member);
  }

  [Fact]
  public async Task Delete_Member_NotFound_When_Id_NonExisting()
  {
    var nonExistingMember = await Client.DeleteAsync("/members/6566");
    await nonExistingMember.ShouldHaveStatusCode(HttpStatusCode.NotFound);
  }
}