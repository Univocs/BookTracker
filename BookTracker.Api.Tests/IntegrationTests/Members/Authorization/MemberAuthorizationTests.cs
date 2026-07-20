using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Members.CreateMember;
using BookTracker.Api.Application.Members.UpdateMember;
using BookTracker.Api.Domain.Books;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.Members.Authorization;

public class MemberAuthorizationTests : IntegrationTest
{
  [Fact]
  public async Task Create_Member_Does_Not_Require_Authentication()
  {
    var request = new CreateMemberRequest
    {
      Name = "Grace Hopper",
      Email = "grace@example.com",
      Password = "debugging-moth"
    };

    var response = await Client.PostAsJsonAsync("/members", request);

    await response.ShouldHaveStatusCode(HttpStatusCode.Created);
  }

  //-------------------------------------------------------------------

  [Fact]
  public async Task Update_Member_Requires_Authentication()
  {
    var memberId = SeedMember("Grace Hopper", "grace@example.com");

    var request = new UpdateMemberRequest
    {
      Name = "Ada Byron",
      Email = "ada.byron@example.com"
    };

    var response = await Client.PutAsJsonAsync(
                      $"/members/{memberId}",
                      request);

    await response.ShouldHaveStatusCode(
        HttpStatusCode.Unauthorized);
  }

  //-------------------------------------------------------------------

  [Fact]
  public async Task Delete_Member_Requires_Authentication()
  {
    var memberId = SeedMember("Grace Hopper", "grace@example.com");
    var deletedMember = await Client.DeleteAsync($"/members/{memberId}");
    await deletedMember.ShouldHaveStatusCode(HttpStatusCode.Unauthorized);
  }

  //-------------------------------------------------------------------

  [Fact]
  public async Task Member_Can_Update_Own_Account()
  {
    var memberId = await AuthenticateAsMember();

    var request = new UpdateMemberRequest
    {
      Name = "Ada Byron",
      Email = "ada.byron@example.com"
    };

    var response = await Client.PutAsJsonAsync(
            $"/members/{memberId}",
            request);

    await response.ShouldHaveStatusCode(
        HttpStatusCode.NoContent);
  }

  //----------------------------------------------------------------------

  [Fact]
  public async Task Member_Cannot_Update_Another_Member()
  {
    var currentMemberId = await AuthenticateAsMember();

    var otherMemberId = SeedMember("Grace Hopper", "grace@example.com");

    var request = new UpdateMemberRequest
    {
      Name = "Changed Name",
      Email = "changed@example.com"
    };

    var response = await Client.PutAsJsonAsync(
                    $"/members/{otherMemberId}",
                    request);

    await response.ShouldHaveStatusCode(
        HttpStatusCode.Forbidden);

    var member =
        Reader.Query(db =>
            db.Members.Find(otherMemberId));

    Assert.NotNull(member);
    Assert.Equal("Grace Hopper", member.Name.Value);
    Assert.Equal("grace@example.com", member.Email.Value);
  }

  //--------------------------------------------------------------------

  [Fact]
  public async Task Member_Cannot_Delete_Another_Member()
  {
    var currentMemberId = await AuthenticateAsMember();

    var otherMemberId = SeedMember("Grace Hopper", "grace@example.com");

    var response = await Client.DeleteAsync($"/members/{otherMemberId}");
    await response.ShouldHaveStatusCode(HttpStatusCode.Forbidden);
  }

  //-------------------------------------------------------------------

  private int SeedMember(string name, string email)
  {
    var member = new Member
    {
      Name = new MemberName(name),
      Email = new MemberEmail(email),
      PasswordHash = "test-password-hash"
    };

    Writer.Seed(db => db.Members.Add(member));
    return member.Id;
  }
}