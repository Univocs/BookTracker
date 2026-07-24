using BookTracker.Api.Domain.Members;
using BookTracker.Api.Domain.Actors;

namespace BookTracker.Api.Domain.Books;

public static class BookPermissions // Actor checks before HTTP requests
{
  // Checks if Administrator to manage books!
  // If member, throw
  public static void EnsureCanManage(Actor actor)
  {
    if (actor.Role == MemberRole.Administrator) return;
    throw new ForbiddenOperationException(
                "This actor cannot manage books.");
  }
}