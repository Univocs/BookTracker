using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Members.CreateMember;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.Members.CreateMember;

public class CreateMemberTests : IntegrationTest
{
  [Fact]
  public async Task Post_Member_Creates_Member()
  {
    var request = new CreateMemberRequest
    {
      Name = "Liam Neeson",
      Email = "Liam@hotmail.com"
    };

    var response = await Client.PostAsJsonAsync("/members", request);
    var created = await response.ReadJsonAs<CreateMemberResponse>(HttpStatusCode.Created);
    Assert.NotNull(created);

    var postedMember = Reader.Query(context => context.Find<Member>(created.Id));

    Assert.NotNull(postedMember);
    Assert.Equal("Liam Neeson", postedMember.Name.Value);
    Assert.Equal("Liam@hotmail.com", postedMember.Email.Value);
  }

  [Fact]
  public async Task  Post_Member_Returns_BadRequest_When_Name_IsEmpty()
  {
    var request = new CreateMemberRequest
    {
      Name = "",
      Email = "Liam@hotmail.com"
    };

    var response = await Client.PostAsJsonAsync("/members", request);
    await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
  }

  [Fact]
  public async Task Post_Member_Returns_BadRequest_When_Email_IsInvalid()
  {
    var request = new CreateMemberRequest
    {
      Name = "Liam Neeson",
      Email = "Liam_hotmail.com"
    };

    var response = await Client.PostAsJsonAsync("/members", request);
    await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
  }
}