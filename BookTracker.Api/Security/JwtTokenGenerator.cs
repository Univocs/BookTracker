using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookTracker.Api.Application.Auth.Login;
using BookTracker.Api.Domain.Members;
using Microsoft.IdentityModel.Tokens;

namespace BookTracker.Api.Security;

public class JwtTokenGenerator(JwtSettings settings)
{
  public LoginResponse Generate(Member member)
  {
    var expiresAt = DateTime.UtcNow.AddMinutes(settings.ExpirationMinutes);

    var claims = new List<Claim>
    { // The payload or facts about the member readable inside the token
      new(ClaimTypes.NameIdentifier, member.Id.ToString()), // claims always strings
      new(ClaimTypes.Name, member.Name.Value),
      new(ClaimTypes.Email, member.Email.Value),
      new(ClaimTypes.Role, member.Role.ToString())
    };

    // SymmetricSecurityKey needs SigningKey to be raw bytes, not string
    // SymmetricSecurityKey both signs and verifies
    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SigningKey));

    // Pairs signingKey with an algorithm --> HMAC-SHA256 is secure choice for symmetric JWT
    var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

    // This builds the in-memory token object — header, payload, and signature (pieces above)
    var token = new JwtSecurityToken(
        issuer: settings.Issuer,
        audience: settings.Audience,
        claims: claims,
        expires: expiresAt,
        signingCredentials: credentials); // signingKey inside credentials from steps above

    var value = new JwtSecurityTokenHandler().WriteToken(token); // token made from steps above

    return new LoginResponse
    {
      AccessToken = value,
      ExpiresAt = expiresAt
    };
  }
}
