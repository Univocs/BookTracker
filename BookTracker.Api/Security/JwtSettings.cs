namespace BookTracker.Api.Security;

// API logs members in (issuer) and is the only API that checks these tokens (audience)
// That's why it's BookTracker for both in appsettings.json 

public class JwtSettings
{
  public const string SectionName = "Jwt";
  public required string Issuer {get; set;} // This issuer made the token
  public required string Audience {get; set;} // Token was made for this app
  public required string SigningKey {get; set;} // Secret SigningKey for the token
  public int ExpirationMinutes { get; set; } = 60; // valid for 60 minutes
}