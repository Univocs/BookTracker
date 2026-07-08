using BookTracker.Api.Application.BookList;
using BookTracker.Api.Application.CreateBook;
using BookTracker.Api.Application.DeleteBook;
using BookTracker.Api.Application.GetBookById;
using BookTracker.Api.Application.UpdateBook;
using BookTracker.Api.Domain;

namespace BookTracker.Api.Endpoints;

public static class BookEndpoints
{
  public static IEndpointRouteBuilder MapBookEndpoints(this IEndpointRouteBuilder app)
  {
    app.MapGet("/books", GetAllBooks);
    app.MapGet("/books/{id:int}", GetBookById);
    app.MapPost("/books", CreateBook);
    app.MapPut("/books/{id:int}", UpdateBook);
    app.MapDelete("/books/{id:int}", DeleteBook);
    return app;
  }

  public static async Task<IResult> GetAllBooks(GetBookListQuery query)
  {
    var books = await query.Execute();
    // Use bookservice to await GetAllBooks()

    return Results.Ok(books);
  }

  public static async Task<IResult> GetBookById(int id, GetBookByIdQuery query)
  {
    var book = await query.Execute(id);
    if (book is null) return Results.NotFound();

    return Results.Ok(book);
  }

  public static async Task<IResult> CreateBook(CreateBookRequest request, CreateBookCommandHandler  handler)
  {
    try
    {
      var response = await handler.Execute(request);
      return Results.Created($"/books/{response.Id}", response);
    }

    catch (DomainException exception)
    {
      return Results.BadRequest(new { error = exception.Message });
    }
  }

  public static async Task<IResult> UpdateBook(int id, UpdateBookRequest request, UpdateBookCommandHandler handler)
  {
    try
    {
      var updated = await handler.Execute(id, request);
      if (!updated) return Results.NotFound();
      return Results.NoContent(); // already updated, no response needed
    }

    catch (DomainException exception)
    {
      return Results.BadRequest(new { error = exception.Message });
    }
  }

  public static async Task<IResult> DeleteBook(int id, DeleteBookCommandHandler handler)
  {
    var deleted = await handler.Execute(id);
    if (!deleted) return Results.NotFound();
    // If book doesn't exist => API can not delete anything, so NotFound()

    return Results.NoContent();
    // Why NoContent()? Because we don't need a response, we already did service.DeleteBook(id);
  }
}