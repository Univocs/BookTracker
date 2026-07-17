namespace BookTracker.Api.Domain.Books;

// Records get built-in value equality since it represents a value, not id.
public record BookTitle
{
  // const = Fixed number/value that can never change.
  public const int MaxLength = 100;
  public string Value { get; }

  public BookTitle(string value)
  {
    var cleanedValue = value;

     // Empty string or only whitespace will throw a DomainException! 
    if (string.IsNullOrWhiteSpace(cleanedValue)) throw new DomainException("Title is required.");
    cleanedValue = value.Trim();

    // Length of cleaned cannot exceed MaxLength, otherwise will throw a DomainException! 
    if (cleanedValue.Length > MaxLength) throw new DomainException($"Title cannot be longer than {MaxLength} characters.");

    // If everything ok, string Value will be the BookTitle value called "cleaned"! 
    Value = cleanedValue;
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