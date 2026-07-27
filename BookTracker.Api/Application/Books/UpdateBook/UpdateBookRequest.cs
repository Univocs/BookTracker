namespace BookTracker.Api.Application.Books.UpdateBook;

public class UpdateBookRequest
{
  public required string Title { get; set; }
  public required string Author { get; set; }
  public int Year { get; set; }
  public Guid Version { get; set; }
}