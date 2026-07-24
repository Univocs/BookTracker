using BookTracker.Api.Domain.Actors;
using BookTracker.Api.Domain.Members;
using BookTracker.Api.Storage.Members;

namespace BookTracker.Api.Application.Members.DeleteMember;

public class DeleteMemberCommandHandler(IMemberRepository memberRepository) : IHandler
{
  public async Task<bool> Execute(Actor actor, int id)
  {
    MemberPermissions.EnsureCanManage(actor, id);
    return await memberRepository.DeleteAsync(id);
  }
}