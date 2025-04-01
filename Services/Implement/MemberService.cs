using BusinessObject.Entities;
using Core;
using DataAccess.Implement;
using DataAccess.Interface;
using Microsoft.AspNetCore.SignalR;
using Services.Interface;

namespace Services.Implement
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;

        private readonly IHubContext<ChatHub> _hubContext;

        public MemberService(IMemberRepository memberRepository, IHubContext<ChatHub> hubContext)
        {
            _memberRepository = memberRepository;
            _hubContext = hubContext;
        }

        public async Task<bool> AddMember(Member member)
        {
            bool isSuccess = await _memberRepository.AddMember(member);
            await _hubContext.Clients.All.SendAsync("ReceiveMessage");
            return isSuccess;
        }

        public async Task<bool> UpdateMember(Member member)
        {
            bool isSuccess = await _memberRepository.UpdateMember(member);
            await _hubContext.Clients.All.SendAsync("ReceiveMessage");
            return isSuccess;
        }

        public async Task<bool> DeleteMember(int memberId)
        {
            bool result = await _memberRepository.DeleteMember(memberId);
            await _hubContext.Clients.All.SendAsync("ReceiveMessage");
            return result;
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

