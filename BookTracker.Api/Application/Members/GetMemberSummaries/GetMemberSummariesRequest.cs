namespace BookTracker.Api.Application.Members.GetMemberSummaries;

public class GetMemberSummariesRequest
{
  public int? Page { get; set; }
  public int? PageSize { get; set; }
  public string? Search { get; set; }
}
