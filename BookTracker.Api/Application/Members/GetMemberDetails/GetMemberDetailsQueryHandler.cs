using BookTracker.Api.Domain.Actors;
using BookTracker.Api.Domain.Members;
using BookTracker.Api.Storage;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Api.Application.Members.GetMemberDetails;

public class GetMemberDetailsQueryHandler(AppDbContext dbContext) : IHandler
{
  public async Task<GetMemberDetailsResponse?> Execute(Actor actor, int id)
  {
    MemberPermissions.EnsureCanViewDirectory(actor);
    return await dbContext.Members
        .AsNoTracking()
        .Where(member => member.Id == id)
        .Select(member => new GetMemberDetailsResponse
        {
          Id = member.Id,
          Name = member.Name,
          Email = member.Email
        })
        .FirstOrDefaultAsync();
  }
}