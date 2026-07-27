using BookTracker.Api.Application.Books.GetBookSummaries;
using BookTracker.Api.Application.Books.CreateBook;
using BookTracker.Api.Application.Books.DeleteBook;
using BookTracker.Api.Application.Books.GetBookDetails;
using BookTracker.Api.Application.Books.UpdateBook;
using BookTracker.Api.Domain;
using System.Security.Claims;
using BookTracker.Api.Storage.Books;

namespace BookTracker.Api.Endpoints.Books;

public static class BookEndpoints
{
  public static IEndpointRouteBuilder MapBookEndpoints(this IEndpointRouteBuilder app)
  {
    // Get books are for public viewing (decision) -> no authorization needed.
    app.MapGet("/books", GetBookSummaries);
    app.MapGet("/books/{id:int}", GetBookDetails);

    // Create, Edit && Delete need authorization trough valid login from member
    app.MapPost("/books", CreateBook)
       .RequireAuthorization();
    app.MapPut("/books/{id:int}", UpdateBook)
       .RequireAuthorization();
    app.MapDelete("/books/{id:int}", DeleteBook)
       .RequireAuthorization();

    return app;
  }

  public static async Task<IResult> GetBookSummaries([AsParameters] GetBookSummariesRequest request, GetBookSummariesQueryHandler query)
  // [AsParameters] tells ASP.NET to fill one object with all of them automatically.
  // It's like a label stuck on one box saying "fill me from the URL" (the request)
  // ASP.Net auto makes this object -> new GetBookListRequest { Page = 2, PageSize = 5 }
  {
    var books = await query.Execute(request);

    return Results.Ok(books);
  }

  public static async Task<IResult> GetBookDetails(int id, GetBookDetailsQueryHandler query)
  {
    var book = await query.Execute(id);
    if (book is null) return Results.NotFound();

    return Results.Ok(book);
  }

  public static async Task<IResult> CreateBook(CreateBookRequest request,
                                               ClaimsPrincipal principal,
                                               CreateBookCommandHandler handler)
  {
    try
    {
      var actor = principal.ToActor();
      var response = await handler.Execute(actor, request);
      return Results.Created($"/books/{response.Id}", response);
    }
    catch (ForbiddenOperationException)
    {
      return Results.Forbid();
    }
    catch (DomainException exception)
    {
      return Results.BadRequest(new { error = exception.Message });
    }
  }

  public static async Task<IResult> UpdateBook(int id,
                                               UpdateBookRequest request,
                                               ClaimsPrincipal principal,
                                               UpdateBookCommandHandler handler)
  {
    try
    {
      var result = await handler.Execute(principal.ToActor(), id, request);
      return result switch
      {
        UpdateBookResult.Updated => Results.NoContent(),
        UpdateBookResult.NotFound => Results.NotFound(),
        UpdateBookResult.Conflict => Results.Conflict(
            new { error = "The book was changed by another user." }),
        _ => throw new ArgumentOutOfRangeException()
      };
    }
    catch (ForbiddenOperationException)
    {
      return Results.Forbid();
    }
    catch (DomainException exception)
    {
      return Results.BadRequest(new { error = exception.Message });
    }
  }

  public static async Task<IResult> DeleteBook(int id,
                                               ClaimsPrincipal principal,
                                               DeleteBookCommandHandler handler)
  {
    try
    {
      var actor = principal.ToActor();
      var deleted = await handler.Execute(actor, id);
      if (!deleted) return Results.NotFound();
      // If book doesn't exist => API can not delete anything, so NotFound()
      return Results.NoContent();
      // Why NoContent()? Because we don't need a response, we already did service.DeleteBook(id);
    }
    catch (ForbiddenOperationException)
    {
      return Results.Forbid();
    }
  }
}