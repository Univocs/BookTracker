namespace BookTracker.Api.Domain.Members;

public class Member
{
  public int Id { get; set; }
  public required MemberName Name { get; set; }
  public required MemberEmail Email { get; set; }

  // Entity does not contain password property (in CreateMemberRequest), only hash!
  public string PasswordHash { get; set; } = string.Empty;
}
