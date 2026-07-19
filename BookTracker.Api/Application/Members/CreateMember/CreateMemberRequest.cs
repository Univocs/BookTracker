namespace BookTracker.Api.Application.Members.CreateMember;

public class CreateMemberRequest
{
  public required string Name { get; set; }
  public required string Email { get; set; }

  // password only created when member is created. Not response.
  public required string Password { get; set; }
}