namespace BookTracker.Api.Domain.Members;

// Value Object of Entity Member.

public record MemberName
{
  public const int MaxLength = 100;
  public string Value { get; }
//------------------------------------------------------------------
  public MemberName(string value)
  {
    var cleaned = value.Trim();

    if (string.IsNullOrWhiteSpace(cleaned)) throw new DomainException("Member name is required.");
    if (cleaned.Length > MaxLength) throw new DomainException($"Member name cannot be longer than {MaxLength} characters.");

    Value = cleaned;
  }
  //------------------------------------------------------------------
  public static implicit operator string(MemberName name)
  {
    return name.Value;
  } // The value can now be used as string => "member.name.value" becomes "member.name"
  //------------------------------------------------------------------
  public override string ToString()
  {
    return Value;
  }
}