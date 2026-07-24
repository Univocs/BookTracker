using System.Security.Claims;
using BookTracker.Api.Application.Members;
using BookTracker.Api.Application.Members.CreateMember;
using BookTracker.Api.Application.Members.DeleteMember;
using BookTracker.Api.Application.Members.GetMemberDetails;
using BookTracker.Api.Application.Members.GetMemberSummaries;
using BookTracker.Api.Application.Members.UpdateMember;
using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Members;
using BookTracker.Api.Security;

namespace BookTracker.Api.Endpoints.Members;

public static class MemberEndpoints
{
  public static IEndpointRouteBuilder MapMemberEndpoints(this IEndpointRouteBuilder app)
  {
    // Get Members && create new member are for public viewing -> no authorization needed.
    app.MapGet("/members", GetMemberSummaries)
      .RequireAuthorization(
        AuthorizationPolicies.ManageMembers);
    app.MapGet("/members/{id:int}", GetMemberDetails)
    .RequireAuthorization(
        AuthorizationPolicies.ManageMembers);
    app.MapPost("/members", CreateMember);

    // Only authorized logged-in member can Edit && Delete
    app.MapPut("/members/{id:int}", UpdateMember)
       .RequireAuthorization();
    app.MapDelete("/members/{id:int}", DeleteMember)
       .RequireAuthorization();

    return app;
  }

  public static async Task<IResult> GetMemberSummaries([AsParameters] GetMemberSummariesRequest request, GetMemberSummariesQueryHandler query)
  {
    var members = await query.Execute(request);
    return Results.Ok(members);
  }

  public static async Task<IResult> GetMemberDetails(int id, GetMemberDetailsQueryHandler query)
  {
    var member = await query.Execute(id);
    if (member is null) return Results.NotFound();
    return Results.Ok(member);
  }

  public static async Task<IResult> CreateMember(CreateMemberRequest request, CreateMemberCommandHandler handler)
  {
    try
    {
      var newMember = await handler.Execute(request);

      return Results.Created($"/members/{newMember.Id}", newMember);
    }
    catch (MemberEmailAlreadyExistsException exception)
    {
      return Results.Conflict(new { error = exception.Message });
    }
    catch (DomainException exception)
    {
      return Results.BadRequest(new { error = exception.Message });
    }
  }

  public static async Task<IResult> UpdateMember(int id,
                                                 UpdateMemberRequest request,
                                                 ClaimsPrincipal user,
                                                 UpdateMemberCommandHandler handler)
  {
    // If member is not the user Id member, forbid him from editing the member.
    if (!CanManageMember(user, id)) return Results.Forbid();

    try
    {
      var updatedMember = await handler.Execute(id, request);
      if (!updatedMember) return Results.NotFound();
      return Results.NoContent();
    }
    catch (MemberEmailAlreadyExistsException exception)
    {
      return Results.Conflict(new { error = exception.Message });
    }
    catch (DomainException exception)
    {
      return Results.BadRequest(new { error = exception.Message });
    }
  }

  public static async Task<IResult> DeleteMember(int id,
                                                 ClaimsPrincipal user,
                                                 DeleteMemberCommandHandler handler)
  {
    if (!CanManageMember(user, id)) return Results.Forbid();

    var memberToDelete = await handler.Execute(id);
    if (!memberToDelete) return Results.NotFound();
    return Results.NoContent();
  }

  // Only the logged-in user is allowed to touch this specific member's data
  private static bool CanManageMember(ClaimsPrincipal user, int memberId)
  {
    // Only true if it's the Administrator or when a member uses his own Id. (for update & delete)
    if (user.IsInRole(nameof(MemberRole.Administrator))) return true; // Administrator

    var claim = user.FindFirstValue(ClaimTypes.NameIdentifier);

    return int.TryParse(claim, out var currentMemberId) && currentMemberId == memberId; // Member himself
  }
  // int.TryParse attempts to convert the claim string into an int. 
  // Unlike int.Parse, which throws if the string isn't a valid number, TryParse never throws 
  // A boolean is returned, and hands you the actual parsed number through an out parameter.
}