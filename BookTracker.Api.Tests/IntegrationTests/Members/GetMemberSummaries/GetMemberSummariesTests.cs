using System.Net;
using BookTracker.Api.Application;
using BookTracker.Api.Application.Members.GetMemberSummaries;
using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.Members.GetMemberSummaries;

public class GetMemberSummariesTests : IntegrationTest
{
  [Fact]
  public async Task Get_Member_Summaries_Returns_Members_With_Paging()
  {
    Writer.Seed(db => db.Members.Add(
      new Member
      {
        Name = new MemberName("Liam Neeson"),
        Email = new MemberEmail("Liam@hotmail.com"),
        PasswordHash = "test-password-hash"
      }
    ));

    var response = await Client.GetAsync("/members");
    var allMembers = await response.ReadJsonAs<PagedResult<MemberSummary>>(HttpStatusCode.OK);
    Assert.NotNull(allMembers);
    var membersInfo = Assert.Single(allMembers.Items);

    Assert.Equal("Liam Neeson", membersInfo.Name);
    Assert.Equal("liam@hotmail.com", membersInfo.Email);
    Assert.Equal(1, allMembers.Page);
    Assert.Equal(10, allMembers.PageSize);
    Assert.Equal(1, allMembers.TotalItems);
    Assert.Equal(1, allMembers.TotalPages);
  }

  [Fact]
  public async Task Get_Member_Summaries_Can_Search_By_Name()
  {
    Writer.Seed(db => db.Members.AddRange(
      new Member
      {
        Name = new MemberName("Liam Neeson"),
        Email = new MemberEmail("Liam@hotmail.com"),
        PasswordHash = "test-password-hash"
      }
    ));

    var response = await Client.GetAsync("/members?search=Liam");
    var result = await response.ReadJsonAs<PagedResult<MemberSummary>>(HttpStatusCode.OK);
    var member = Assert.Single(result.Items);

    Assert.Equal("Liam Neeson", member.Name);
    Assert.Equal("liam@hotmail.com", member.Email);
    Assert.Equal(1, result.TotalItems);
    Assert.Equal(1, result.TotalPages);
  }

  [Fact]
  public async Task Get_Member_Summaries_Can_Search_By_Email()
  {
    Writer.Seed(db => db.Members.AddRange(
     new Member
     {
       Name = new MemberName("Liam Neeson"),
       Email = new MemberEmail("someone@hotmail.com"),
       PasswordHash = "test-password-hash"
     }
   ));

    var response = await Client.GetAsync("/members?search=someone");
    var result = await response.ReadJsonAs<PagedResult<MemberSummary>>(HttpStatusCode.OK);
    var member = Assert.Single(result.Items);

    Assert.Equal("Liam Neeson", member.Name);
    Assert.Equal("someone@hotmail.com", member.Email);
    Assert.Equal(1, result.TotalItems);
    Assert.Equal(1, result.TotalPages);
  }

  [Fact]
  public async Task Get_Member_Summaries_Applies_Paging_After_Search()
  {
    Writer.Seed(db =>
    {
      db.Members.AddRange(
          new Member
          {
            Name = new MemberName("Liam Neeson"),
            Email = new MemberEmail("someone@hotmail.com"),
            PasswordHash = "test-password-hash"
          },
          new Member
          {
            Name = new MemberName("john"),
            Email = new MemberEmail("someone_Charlie@hotmail.com"),
            PasswordHash = "test-password-hash"
          },
         new Member
         {
           Name = new MemberName("John Neeson"),
           Email = new MemberEmail("someone_John@hotmail.com"),
           PasswordHash = "test-password-hash"
         });
    });

    var response = await Client.GetAsync("/members?search=john&page=2&pageSize=1");
    var result = await response.ReadJsonAs<PagedResult<MemberSummary>>(HttpStatusCode.OK);

    var member = Assert.Single(result.Items);

    Assert.Equal("John Neeson", member.Name);
    Assert.Equal("someone_john@hotmail.com", member.Email);
    Assert.Equal(2, result.Page);
    Assert.Equal(1, result.PageSize);
    Assert.Equal(2, result.TotalItems);
    Assert.Equal(2, result.TotalPages);
  }

  
  [Fact]
  public async Task Get_Member_Summaries_Returns_Empty_List_When_Search_NoResults()
  {
    Writer.Seed(db =>
    {
      db.Members.AddRange(
          new Member
          {
            Name = new MemberName("Liam Neeson"),
            Email = new MemberEmail("someone@hotmail.com"),
            PasswordHash = "test-password-hash"
          });
    });

    var response = await Client.GetAsync("/books?search=Raymond");
    var result = await response.ReadJsonAs<PagedResult<MemberSummary>>(HttpStatusCode.OK);

    Assert.NotNull(result);
    Assert.Empty(result.Items);
  }

  [Fact]
    public void BookTitle_And_BookAuthor_RejectsNull()
    {
        var nameException = Assert.Throws<DomainException>(() => new MemberName(null!));
        var emailException = Assert.Throws<DomainException>(() => new MemberEmail(null!));

        Assert.Equal("Member name is required.", nameException.Message);
        Assert.Equal("Member email is required.", emailException.Message);
    }
}