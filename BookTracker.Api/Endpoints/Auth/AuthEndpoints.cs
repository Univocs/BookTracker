using System.Security.Claims;
using BookTracker.Api.Application.Auth.GetCurrentMember;
using BookTracker.Api.Application.Auth.Login;

namespace BookTracker.Api.Endpoints.Auth;

public static class AuthEndpoints
{
  public static IEndpointRouteBuilder MapAuthEndpoints(
           this IEndpointRouteBuilder app)
  {
    app.MapPost("/auth/login", Login);
    app.MapGet("/auth/me", GetCurrentMember).RequireAuthorization();
    // RequireAuthorization secures endpoint, invalid token -> 401 Unauthorized

    return app;
  }

  private static async Task<IResult> Login(
                 LoginRequest request,        // email && password
                 LoginCommandHandler handler) // generates LoginResponse
  {
    var response = await handler.Execute(request);
    if (response is null) return Results.Unauthorized();
    return Results.Ok(response);
  }

  // playload contains valid claims of token for endpoint to read current member
  private static IResult GetCurrentMember(ClaimsPrincipal user)
  {
    // retrieval of member claim set during token generation
    // !.Value tells te compiler, trust me, this can't be null
    var id = user.FindFirst(ClaimTypes.NameIdentifier)!.Value;
    var name = user.FindFirst(ClaimTypes.Name)!.Value;
    var email = user.FindFirst(ClaimTypes.Email)!.Value;

    return Results.Ok(new CurrentMemberResponse
    {
      Id = int.Parse(id), // Id parsed from string -> int for response
      Name = name,
      Email = email
    });
  }
}