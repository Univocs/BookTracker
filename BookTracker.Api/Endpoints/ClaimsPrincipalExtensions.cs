using System.Security.Claims;
using BookTracker.Api.Domain.Members;
using BookTracker.Api.Domain.Actors;

namespace BookTracker.Api.Endpoints;

public static class ClaimsPrincipalExtensions
{
  // principal returns ID && Role if valid!
  // only this method knows the name and structure of the claims.
  public static Actor ToActor(this ClaimsPrincipal principal) // Extending ClaimsPrincipal
  {
    // Pull member id out of the token's claim
    var memberIdValue = principal.FindFirstValue(ClaimTypes.NameIdentifier);
    // Also pull the member role out of the token's claim
    var roleValue = principal.FindFirstValue(ClaimTypes.Role);

    if (!int.TryParse(memberIdValue, out var memberId))
    { // try to parse id string to int, throw if no valid id claim
      throw new InvalidOperationException(
                "Authenticated user has no valid member id.");
    }

    if (!Enum.TryParse<MemberRole>(roleValue, out var role))
    { // try to parse id string to int, throw if no valid role claim
      throw new InvalidOperationException(
                "Authenticated user has no valid member role.");
    }

    return new Actor(memberId, role);
  }
}