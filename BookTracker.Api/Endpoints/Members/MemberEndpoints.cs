using System.Security.Claims;
using BookTracker.Api.Application.Members;
using BookTracker.Api.Application.Members.CreateMember;
using BookTracker.Api.Application.Members.DeleteMember;
using BookTracker.Api.Application.Members.GetMemberDetails;
using BookTracker.Api.Application.Members.GetMemberSummaries;
using BookTracker.Api.Application.Members.UpdateMember;
using BookTracker.Api.Domain;

namespace BookTracker.Api.Endpoints.Members;

public static class MemberEndpoints
{
  public static IEndpointRouteBuilder MapMemberEndpoints(this IEndpointRouteBuilder app)
  {
    // Get Members && create new member are for public viewing -> no authorization needed.
    app.MapGet("/members", GetMemberSummaries)
      .RequireAuthorization();
    app.MapGet("/members/{id:int}", GetMemberDetails)
    .RequireAuthorization();
    app.MapPost("/members", CreateMember);

    // Only authorized logged-in member can Edit && Delete
    app.MapPut("/members/{id:int}", UpdateMember)
       .RequireAuthorization();
    app.MapDelete("/members/{id:int}", DeleteMember)
       .RequireAuthorization();

    return app;
  }

  public static async Task<IResult> GetMemberSummaries([AsParameters]
                                                       GetMemberSummariesRequest request,
                                                       ClaimsPrincipal principal,
                                                       GetMemberSummariesQueryHandler query)
  {
    try
    {
      var actor = principal.ToActor();
      var members = await query.Execute(actor, request);
      return Results.Ok(members);
    }
    catch (ForbiddenOperationException)
    {
      return Results.Forbid();
    }
  }

  public static async Task<IResult> GetMemberDetails(int id,
                                                     ClaimsPrincipal principal,
                                                     GetMemberDetailsQueryHandler query)
  {
    try
    {
      var actor = principal.ToActor();
      var member = await query.Execute(actor, id);
      if (member is null) return Results.NotFound();
      return Results.Ok(member);
    }
    catch (ForbiddenOperationException)
    {
      return Results.Forbid();
    }
  }

  public static async Task<IResult> CreateMember(CreateMemberRequest request,
                                                 CreateMemberCommandHandler handler)
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
                                                 ClaimsPrincipal principal,
                                                 UpdateMemberCommandHandler handler)
  {
    try
    {
      var actor = principal.ToActor();
      var updatedMember = await handler.Execute(actor, id, request);
      if (!updatedMember) return Results.NotFound();
      return Results.NoContent();
    }
    catch (ForbiddenOperationException)
    {
      return Results.Forbid();
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
                                                 ClaimsPrincipal principal,
                                                 DeleteMemberCommandHandler handler)
  {
    try
    {
      var actor = principal.ToActor();
      var memberToDelete = await handler.Execute(actor, id);
      if (!memberToDelete) return Results.NotFound();
      return Results.NoContent();
    }
    catch (ForbiddenOperationException)
    {
      return Results.Forbid();
    }
    catch (DomainException exception)
    {
      return Results.BadRequest(
          new { error = exception.Message });
    }
  }
}