namespace BookTracker.Api.Application.Books.CreateBook;

public class CreateBookResponse
{
  public int Id { get; set; }
  public required string Title {get; set;}
  public required string Author {get; set;}
  public int Year {get; set;}
}