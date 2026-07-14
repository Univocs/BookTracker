namespace BookTracker.Api.Application.Books.GetBookSummaries;

public class GetBookSummariesRequest
{
  public int? Page { get; set; }
  public int? PageSize { get; set; }
  public string? Search { get; set; }
}
      /*

        The order is important:

          1)    Filter first.
          2)    Then count.
          3)    Then sort.
          4)    Then apply paging.
          5)    Then project to DTOs.
          
      */