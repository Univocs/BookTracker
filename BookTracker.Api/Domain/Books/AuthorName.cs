namespace BookTracker.Api.Domain.Books;

// Records get built-in value equality since it represents a value, not id.
public record AuthorName
{
  // const = Fixed number/value that can never change.
  public const int MaxLength = 100;
  public string Value { get; }

  public AuthorName(string value)
  {
    // Remove leading/trailing whitespace first from value!
    var cleaned = value.Trim();

    // Empty string or only whitespace will throw a DomainException! 
    if (string.IsNullOrWhiteSpace(cleaned)) throw new DomainException("Author is required.");

    // Length of cleaned cannot exceed MaxLength, otherwise will throw a DomainException! 
    if (cleaned.Length > MaxLength) throw new DomainException($"Author cannot be longer than {MaxLength} characters.");

    // If everything ok, string Value will be the AuthorName value called "cleaned"! 
    Value = cleaned;
  }

  // implicit operator => whenever string expected, automatically convert from object to string 
  public static implicit operator string(AuthorName author)
  {
    return author.Value; // Use Case => var title = new BookTitle("Dune");  =>  string text = title;
  }                      // Domain Rules still exist even in string form with possible exceptions.
  

  // override ToString() gives the actual text or value of the title instead of the class name.
  public override string ToString()
  {
    return Value;
  }
}