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
    app.MapGet("/members", GetMemberSummaries);
    app.MapGet("/members/{id:int}", GetMemberDetails);
    app.MapPost("/members", CreateMember);
    app.MapPut("/members/{id:int}", UpdateMember);
    app.MapDelete("/members/{id:int}", DeleteMember);
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

  public static async Task<IResult> UpdateMember(int id, UpdateMemberRequest request, UpdateMemberCommandHandler handler)
  {
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

  public static async Task<IResult> DeleteMember(int id, DeleteMemberCommandHandler handler)
  {
    var memberToDelete = await handler.Execute(id);
    if (!memberToDelete) return Results.NotFound();

    return Results.NoContent();
  }
}