using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Members;
using BookTracker.Api.Storage.Members;
using Microsoft.AspNetCore.Identity;

namespace BookTracker.Api.Application.Members.CreateMember;

public class CreateMemberCommandHandler(IMemberRepository memberRepository, IPasswordHasher<Member> passwordHasher) : IHandler
{                                                                        // IPasswordHasher initialized through the WebApplicationBuilderExtensions
  public async Task<CreateMemberResponse> Execute(CreateMemberRequest request)
  {
    var name = new MemberName(request.Name);
    var email = new MemberEmail(request.Email);

    if (string.IsNullOrWhiteSpace(request.Password)) throw new DomainException("Password is required.");
    if (request.Password.Length < 8) throw new DomainException("Password must contain at least 8 characters.");

    if (await memberRepository.EmailExistsAsync(email)) throw new MemberEmailAlreadyExistsException();

    var newMember = new Member { Name = name, Email = email };

    newMember.PasswordHash = passwordHasher.HashPassword(newMember, request.Password);

    var savedNewMember = await memberRepository.AddAsync(newMember);
    
    return new CreateMemberResponse
    {
      Id = savedNewMember.Id,
      Name = savedNewMember.Name,
      Email = savedNewMember.Email
    };
  }
}