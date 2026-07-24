using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Actors;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.Domain.Members;

public class MemberPermissionsTests
{
  [Fact]
  public void Member_Can_Manage_Own_Account()
  {
    var actor = new Actor(42, MemberRole.Member);
    MemberPermissions.EnsureCanManage(actor, 42);
  }

  [Fact]
  public void Member_Cannot_Manage_Another_Account()
  {
    var actor = new Actor(42, MemberRole.Member);

    Assert.Throws<ForbiddenOperationException>(() =>
            MemberPermissions.EnsureCanManage(actor, 99));
  }
}