using BusinessObject.Entities;
using Core;
using DataAccess.Interface;
using Services.Interface;

namespace Services.Implement
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;

        public MemberService(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        public async Task<bool> AddMember(Member member)
        {
            return await _memberRepository.AddMember(member);
        }

        public async Task<bool> UpdateMember(Member member)
        {
            return await _memberRepository.UpdateMember(member);
        }

        public async Task<bool> DeleteMember(int memberId)
        {
            return await _memberRepository.DeleteMember(memberId);
        }

        public async Task<Member> GetMember(int memberId)
        {
            return await _memberRepository.GetMember(memberId);
        }

        public async Task<PaginatedList<Member>> GetMembers(int pageNumber, int pageSize)
        {
            return await _memberRepository.GetMembers(pageNumber, pageSize);
        }

        public async Task<PaginatedList<Member>> GetMembers(string email, string companyName, string country, int pageNumber, int pageSize)
        {
            return await _memberRepository.GetMembers(email, companyName, country, pageNumber, pageSize);
        }

        public async Task<Member?> Login(string email, string password)
        {
            return await _memberRepository.Login(email, password);
        }

        public async Task<Member> GetMembersByEmailAddress(string emailAddress)
        {
            return await _memberRepository.GetMembersByEmailAddress(emailAddress);
        }

        public Task<IEnumerable<Member>> GetMembers()
        {
            return _memberRepository.GetMembers();
        }
    }
}

