using System.Net;
using BookTracker.Api.Application.Members.GetMemberDetails;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.Members.GetMemberDetails;

public class GetMemberDetailsTests : IntegrationTest
{
  [Fact]
  public async Task Get_Member_Details_Returns_Existing_Member()
  {
    Writer.Seed(db => db.Members.Add(
      new Member
      {
        Name = new MemberName("Liam Neeson"),
        Email = new MemberEmail("Liam@hotmail.com")
      }
    ));

    var response = await Client.GetAsync("/members/1");
    var member = await response.ReadJsonAs<GetMemberDetailsResponse>(HttpStatusCode.OK);

    Assert.NotNull(member);
    Assert.Equal(1, member.Id);
    Assert.Equal("Liam Neeson", member.Name);
    Assert.Equal("Liam@hotmail.com", member.Email);
  }

  [Fact]
  public async Task Get_Member_Details_NotFound_When_Member_Does_Not_Exist()
  {
    var response = await Client.GetAsync("/members/1");
    await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
  }
}