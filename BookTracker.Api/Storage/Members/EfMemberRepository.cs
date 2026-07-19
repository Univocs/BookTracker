using BookTracker.Api.Domain.Members;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Api.Storage.Members;

public class EfMemberRepository(AppDbContext dbContext) : IMemberRepository
{
  public async Task<Member> AddAsync(Member member)
  {
    dbContext.Members.Add(member);
    await dbContext.SaveChangesAsync();
    return member;
  }

  public async Task<bool> UpdateAsync(Member member)
  {
    var existingMember = await dbContext.Members.FindAsync(member.Id);
    if (existingMember is null) return false;

    existingMember.Email = member.Email;
    existingMember.Name = member.Name;

    await dbContext.SaveChangesAsync();
    return true;
  }

  public async Task<bool> DeleteAsync(int id)
  {
    var member = await dbContext.Members.FindAsync(id);
    if (member is null) return false;

    dbContext.Members.Remove(member);
    await dbContext.SaveChangesAsync();
    return true;
  }

  public async Task<bool> EmailExistsAsync(MemberEmail email, int? memberIdToIgnore = null)
  {
    var query = dbContext.Members.Where(member => member.Email == email);

    if (memberIdToIgnore.HasValue) // needed for update of a member. 
    {
      query = query.Where(member => member.Id != memberIdToIgnore.Value);
    } // keep only members whose Id is not equal to the id I want to ignore.
      // In other words: remove the member I'm currently updating from the results.

    return await query.AnyAsync();
  }
}
