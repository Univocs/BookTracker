namespace BookTracker.Api.Domain.Books;

public class Book
{
  public int Id {get; set;}
  public required BookTitle Title {get; set;}  // value object BookTitle in DTO for exceptions! 
  public required AuthorName Author {get; set;}  // value object AuthorName in DTO for exceptions! 
  public int Year {get; set;}
}