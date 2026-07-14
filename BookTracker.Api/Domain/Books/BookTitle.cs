namespace BookTracker.Api.Domain.Books;

// Records get built-in value equality since it represents a value, not id.
public record BookTitle
{
  // const = Fixed number/value that can never change.
  public const int MaxLength = 100;
  public string Value { get; }

  public BookTitle(string value)
  {
    // Remove leading/trailing whitespace first from value!
    var cleaned = value.Trim();

    // Empty string or only whitespace will throw a DomainException! 
    if (string.IsNullOrWhiteSpace(cleaned)) throw new DomainException("Title is required.");

    // Length of cleaned cannot exceed MaxLength, otherwise will throw a DomainException! 
    if (cleaned.Length > MaxLength) throw new DomainException($"Title cannot be longer than {MaxLength} characters.");

    // If everything ok, string Value will be the BookTitle value called "cleaned"! 
    Value = cleaned;
  }

  // implicit operator => whenever string expected, automatically convert from object to string 
  public static implicit operator string(BookTitle title)
  {
    return title.Value; // returns string value
  }

  // override ToString() gives the actual text or value of the title instead of the class name.
  public override string ToString()
  {
    return Value;
  }
}