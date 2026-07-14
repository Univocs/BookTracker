using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Storage.Members;

public interface IMemberRepository
{
  Task <Member> AddAsync(Member member);
  Task <bool> UpdateAsync(Member member);
  Task <bool> DeleteAsync(int id);
}