namespace BookTracker.Api.Domain.Members;

// Value Object of Entity Member.

public class MemberEmail
{
  public const int MaxLength = 200;
  public string Value { get; }
  //------------------------------------------------------------------
  public MemberEmail(string value)
  {
    var cleaned = value;
    if (string.IsNullOrWhiteSpace(cleaned)) throw new DomainException("Member email is required.");
    cleaned = value.Trim().ToLowerInvariant();
    
    if (cleaned.Length > MaxLength) throw new DomainException($"Member email cannot be longer than {MaxLength} characters.");
    if (!cleaned.Contains('@')) throw new DomainException("Member email needs to contain '@'.");

    Value = cleaned;
  }
  //------------------------------------------------------------------
  public static implicit operator string(MemberEmail email)
  {
    return email.Value;
  } // The value can now be used as string => "member.email.value" becomes "member.email"
  //------------------------------------------------------------------
  public override string ToString()
  {
    return Value;
  }
}