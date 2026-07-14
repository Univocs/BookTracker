using BookTracker.Api.Storage.Members;

namespace BookTracker.Api.Application.Members.DeleteMember;

public class DeleteMemberCommandHandler(IMemberRepository memberRepository) : IHandler
{
  public async Task<bool> Execute(int id)
  {
    return await memberRepository.DeleteAsync(id);
  }
}