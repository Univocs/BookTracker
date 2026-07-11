namespace BookTracker.Api.Application.GetBookDetails;
// DTO
public class GetBookDetailsResponse
{
  public int Id { get; set; }
  public required string Title { get; set; }
  public required string Author { get; set; }
  public int Year { get; set; }
}