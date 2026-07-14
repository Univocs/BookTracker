using BookTracker.Api.Domain.Members;
using BookTracker.Api.Storage.Members;

namespace BookTracker.Api.Application.Members.CreateMember;

public class CreateMemberCommandHandler(IMemberRepository memberRepository) : IHandler
{
  public async Task<CreateMemberResponse> Execute(CreateMemberRequest request)
  {
    var newMember = new Member  // We make a new Member
    {
      Name = new MemberName(request.Name),
      Email = new MemberEmail(request.Email)
    };

    var savedNewMember = await memberRepository.AddAsync(newMember);

    return new CreateMemberResponse
    {
      Id = savedNewMember.Id,
      Name = savedNewMember.Name,
      Email = savedNewMember.Email
    };

  }
}