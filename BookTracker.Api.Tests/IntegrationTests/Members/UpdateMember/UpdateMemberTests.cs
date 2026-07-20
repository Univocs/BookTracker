using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Members.UpdateMember;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.Members.UpdateMember;

public class UpdateMemberTests : IntegrationTest
{
  [Fact]
  public async Task Update_Member_Updates_Member()
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

    var updateRequest = new UpdateMemberRequest
    {
      Name = "Charlie Neeson",
      Email = "charlie@hotmail.com"
    };

    var response = await Client.PutAsJsonAsync($"/members/{memberId}", updateRequest);
    await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

    var updatedMember = Reader.Query(db => db.Members.Find(1));
    Assert.NotNull(updatedMember);
    Assert.Equal("Charlie Neeson", updatedMember.Name);
    Assert.Equal("charlie@hotmail.com", updatedMember.Email);
  }

  [Fact]
  public async Task Update_Member_NotFound_When_Member_DoesNotExist()
  {
    var memberId = await AuthenticateAsMember(); // Create member 
    Writer.Seed(db =>                            // Delete member 
    {
        var member = db.Members.Find(memberId);
        if (member is not null) db.Members.Remove(member);
    });

    var updateRequest = new UpdateMemberRequest
    {
      Name = "Charlie Neeson",
      Email = "charlie@hotmail.com"
    };

    var response = await Client.PutAsJsonAsync($"/members/{memberId}", updateRequest);
    await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task Update_Member_With_Invalid_Email_Gives_Bad_Request()
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

    var updateRequest = new UpdateMemberRequest
    {
      Name = "Charlie Neeson",
      Email = "charlie_hotmail.com"
    };

    var response = await Client.PutAsJsonAsync($"/members/{memberId}", updateRequest);
    await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
  }
}