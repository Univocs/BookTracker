using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Members.CreateMember;
using BookTracker.Api.Domain.Members;
using Microsoft.AspNetCore.Identity;

namespace BookTracker.Api.Tests.IntegrationTests.Members.CreateMember;

public class CreateMemberTests : IntegrationTest
{
  [Fact]
  public async Task Post_Member_Creates_Member()
  {
    var request = new CreateMemberRequest
    {
      Name = "Liam Neeson",
      Email = "Liam@hotmail.com",
      Password = "something_password"
    };

    var response = await Client.PostAsJsonAsync("/members", request);
    var created = await response.ReadJsonAs<CreateMemberResponse>(HttpStatusCode.Created);

    var postedMember = Reader.Query(db => db.Members.Single(current => current.Id == created.Id));
    Assert.NotEqual("something_password", postedMember.PasswordHash);

    var passwordHasher = new PasswordHasher<Member>();

    var result = passwordHasher.VerifyHashedPassword(postedMember, postedMember.PasswordHash, "something_password");

    Assert.Equal(PasswordVerificationResult.Success, result);

    Assert.NotNull(postedMember);
    Assert.Equal("Liam Neeson", postedMember.Name.Value);
    Assert.Equal("liam@hotmail.com", postedMember.Email.Value);
  }

  [Fact]
  public async Task Post_Member_Returns_BadRequest_When_Name_IsEmpty()
  {
    var request = new CreateMemberRequest
    {
      Name = "",
      Email = "Liam@hotmail.com",
      Password = "something_password"
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
      Email = "Liam_hotmail.com",
      Password = "something_password"
    };

    var response = await Client.PostAsJsonAsync("/members", request);
    await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
  }

  [Fact]
  public async Task Post_Members_With_Empty_Password_Return_BadRequest()
  {
    var request = new CreateMemberRequest
    {
      Name = "Ada Lovelace",
      Email = "ada@example.com",
      Password = ""
    };

    var response = await Client.PostAsJsonAsync("/members", request);
    await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
  }

  [Fact]
  public async Task Post_Members_With_Less_Then_8Chars_Return_BadRequest()
  {
    var request = new CreateMemberRequest
    {
      Name = "Ada Lovelace",
      Email = "ada@example.com",
      Password = "pass"
    };

    var response = await Client.PostAsJsonAsync("/members", request);
    await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
  }

  [Fact]
  public async Task Post_Members_With_Double_Mail_Returns_Conflict()
  {
    var posterMember1 = new CreateMemberRequest
    {
      Name = "Adaa Lovelace",
      Email = "ada@example.com",
      Password = "password"
    };

    var responseMember1 = await Client.PostAsJsonAsync("/members", posterMember1);
    await responseMember1.ShouldHaveStatusCode(HttpStatusCode.Created);

    var posterMember2 = new CreateMemberRequest
    {
      Name = "Jacky Chan",
      Email = "ADA@EXAMPLE.COM",
      Password = "password"
    };

    var responseMember2 = await Client.PostAsJsonAsync("/members", posterMember2);
    await responseMember2.ShouldHaveStatusCode(HttpStatusCode.Conflict);
  }
}