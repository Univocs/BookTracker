using BookTracker.Api.Domain.Members;
using BookTracker.Api.Storage.Members;

namespace BookTracker.Api.Application.Members.UpdateMember;

public class UpdateMemberCommandHandler(IMemberRepository memberRepository) : IHandler
{
  public async Task<bool> Execute(int id, UpdateMemberRequest request)
  {
    var updatedMember = new Member
    {
      Id = id,
      Name = new MemberName(request.Name),
      Email = new MemberEmail(request.Email)
    };

    return await memberRepository.UpdateAsync(updatedMember);
  }
}