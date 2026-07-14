using BookTracker.Api.Storage;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Api.Application.Members.GetMemberSummaries;

public class GetMemberSummariesQueryHandler(AppDbContext dbContext) : IHandler
{
  private const int DefaultPages = 1;
  private const int DefaultPageSize = 10;
  private const int MinPageSize = 1;
  private const int MaxPageSize = 50;
  public async Task<GetMemberSummariesResponse> Execute(GetMemberSummariesRequest request)
  {
    var page = Math.Max(1, request.Page ?? DefaultPages);
    var pageSize = Math.Clamp(request.PageSize ?? DefaultPageSize, MinPageSize, MaxPageSize);

    var membersQuery = dbContext.Members.AsNoTracking(); // AsNoTracking only reads

    if (!string.IsNullOrWhiteSpace(request.Search)) // filter only if search was not null/whitespace
    {
      var searched = $"%{request.Search.Trim()}%"; // trim the search request, %% needed as a contain()
      membersQuery = membersQuery.Where(member => // Filtering
          EF.Functions.Like((string)member.Name, searched) || // searched needs to match Name
          EF.Functions.Like((string)member.Email, searched)); // searched needs to match Email
    }
    // total of the membersQuery search
    var totalMembers = await membersQuery.CountAsync();

    var members = await membersQuery  // Fetches members from the Search
        .OrderBy(member => member.Id) // Order by Id
        .Skip((page - 1) * pageSize)  // starts from page 1, new pages every 10 Id's
        .Take(pageSize)               // Grab next 10 for new page
        .Select(member => new MemberSummary
        {
          Id = member.Id,
          Name = member.Name.Value,
          Email = member.Email.Value
        })
        .ToListAsync();                // Put members into a list

    return new GetMemberSummariesResponse
    {
      Items = members,
      Page = page,
      PageSize = pageSize,
      TotalItems = totalMembers,
      TotalPages = (int)Math.Ceiling(totalMembers / (double)pageSize)
      // 42 / 10.0 (dec) = 4.2 → Math.Ceiling always rounds up to the next whole number -> new page
    };
  }
}