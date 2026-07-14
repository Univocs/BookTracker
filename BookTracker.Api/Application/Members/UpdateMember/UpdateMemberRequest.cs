namespace BookTracker.Api.Application.Members.UpdateMember;

public class UpdateMemberRequest
{
  public required string Name { get; set; }
  public required string Email { get; set; }
}