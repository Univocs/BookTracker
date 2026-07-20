using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BookTracker.Api.Application.Auth.Login;
using BookTracker.Api.Domain.Members;
using Microsoft.AspNetCore.Identity;

namespace BookTracker.Api.Tests.IntegrationTests;

// "abstract" = Can never be instantiated directly (no "new IntegrationTest()").
// It only exists to be inherited from, it has no tests of its own.
public abstract class IntegrationTest : IDisposable
{
  private readonly CustomWebApplicationFactory factory = new();

  // "protected" = visible to this class and any subclass, but not to outside code.
  // "{ get; }" = get-only property. Test class can READ Client, but no set (no "Client = something;").
  protected HttpClient Client { get; }
  protected EfReader Reader { get; }
  protected EfWriter Writer { get; }

  // Build all three once, right when the test starts, using the factory.
  protected IntegrationTest()
  {
    Client = factory.CreateClient();
    Reader = factory.GetReader();
    Writer = factory.GetWriter();
  }

  // Required by IDisposable. xUnit automatically calls this after each test
  public void Dispose()
  {
    // Clean up the HttpClient first.
    Client.Dispose();

    // Then dispose the factory itself, the open SQLite in-memory connection.
    factory.Dispose();
  }

  protected async Task<int> AuthenticateAsMember( // Simulates succesful login
      string name = "Ada Lovelace",
      string email = "ada@example.com",
      string password = "analytical-engine")
  {
    // We make a custom member using the parameters! 
    var member = new Member
    {
      Name = new MemberName(name),
      Email = new MemberEmail(email),
      PasswordHash = string.Empty
    };

    // We hash the empty password of member with the passwordHasher
    var passwordHasher = new PasswordHasher<Member>();
    member.PasswordHash = passwordHasher.HashPassword(member, password);

    Writer.Seed(db => db.Members.Add(member));

    // We try to log in with our new member and the parameters data
    var request = new LoginRequest
    {
      Email = email,
      Password = password
    };

    var response = await Client.PostAsJsonAsync("/auth/login", request);
    var login = await response.ReadJsonAs<LoginResponse>(HttpStatusCode.OK);

    Client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue(
            "Bearer",
            login.AccessToken);

    return member.Id;
  }
}