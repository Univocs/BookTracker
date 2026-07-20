using BookTracker.Api.Domain.Members;
using BookTracker.Api.Security;
using BookTracker.Api.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Api.Application.Auth.Login;

// dbContext finds the member → passwordHasher checks their 
// password is correct → tokenGenerator issues the token

public class LoginCommandHandler
  (AppDbContext dbContext, // Get Member + stored passwordHasher
   IPasswordHasher<Member> passwordHasher, // submitted password = stored passwordHasher??
   JwtTokenGenerator tokenGenerator) : IHandler // Build and sign JWT
{
  public async Task<LoginResponse?> Execute(LoginRequest request)
  {              // Can be null when input not authenticated, no throwing here
    if (string.IsNullOrWhiteSpace(request.Email) ||
        string.IsNullOrWhiteSpace(request.Password)) return null;

    var email = request.Email.Trim().ToLowerInvariant();

    var member = await dbContext.Members
                      .AsNoTracking() // only reads
                      .SingleOrDefaultAsync(m => (string)m.Email == email);
                      // one member matches (unique index) with this email, or null

    if (member is null) return null; // if OrDefault (is null) => bail method, return null

    // Reads the existing hash and checks if the password input would have produced the hash
    var verification = passwordHasher.VerifyHashedPassword(member, member.PasswordHash, request.Password);
    if (verification == PasswordVerificationResult.Failed) return null; 
    
    return tokenGenerator.Generate(member);
  }
}