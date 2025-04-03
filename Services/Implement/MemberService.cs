using AutoMapper;
using BusinessObject.Entities;
using Core;
using DataAccess.Implement;
using DataAccess.Interface;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Services.Interface;
using Services.Models.DTO;
using System.Net.Mail;

namespace Services.Implement
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;

        private readonly IHubContext<ChatHub> _hubContext;


        private readonly IMapper _mapper;

        private readonly ILogger<MemberService> _logger;

        public MemberService(IMemberRepository memberRepository, IHubContext<ChatHub> hubContext, IMapper mapper, ILogger<MemberService> logger)
        {
            _memberRepository = memberRepository;
            _hubContext = hubContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> AddMember(MemberDTO memberDto)
        {
            memberDto.Password = this.HashPassword(memberDto.Password);

            var member = _mapper.Map<Member>(memberDto);

            var member1 = await _memberRepository.AddMember(member);

            await _hubContext.Clients.All.SendAsync("ReceiveMessage");

            return member1;
        }
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public async Task<bool> UpdateMember(MemberDTO memberDto)
        {
            if (!string.IsNullOrWhiteSpace(memberDto.Password))
            {
                memberDto.Password = this.HashPassword(memberDto.Password);
            }
            
            else if (string.IsNullOrWhiteSpace(memberDto.Password))
            {
                var memberOld = await _memberRepository.GetMember(memberDto.MemberId);
                memberDto.Password = memberOld.Password;
            }

            var member = _mapper.Map<Member>(memberDto);

            var member1 = await _memberRepository.UpdateMember(member);

            await _hubContext.Clients.All.SendAsync("ReceiveMessage");

            return member1;
        }

        public async Task<bool> DeleteMember(int memberId)
        {

            bool result = await _memberRepository.DeleteMember(memberId);
            await _hubContext.Clients.All.SendAsync("ReceiveMessage");
            return result;
        }

        public async Task<MemberDTO> GetMember(int memberId)
        {
            var member = await _memberRepository.GetMember(memberId);

            if (member == null)
            {
                _logger.LogWarning($"Member with ID {memberId} not found.");
                return null;
            }
            var memberDto = _mapper.Map<MemberDTO>(member);

            return memberDto;
        }

        public async Task<PaginatedList<MemberDTO>> GetMembers(int pageNumber, int pageSize)
        {
            var member = await _memberRepository.GetMembers(pageNumber, pageSize);
            if (member == null)
            {
                _logger.LogWarning("No members found for the given parameters.");
                return null;
            }
            var items = _mapper.Map<List<MemberDTO>>(member.Items);
            return new PaginatedList<MemberDTO>(items, member.TotalCount, pageNumber, pageSize);
        }

        public async Task<PaginatedList<MemberDTO>> GetMembers(string email, string companyName, string country, int pageNumber, int pageSize)
        {
            var member = await _memberRepository.GetMembers(email, companyName, country, pageNumber, pageSize);
            if (member == null)
            {
                _logger.LogWarning("No members found for the given parameters.");
                return null;
            }
            var items = _mapper.Map<List<MemberDTO>>(member.Items);

            return new PaginatedList<MemberDTO>(items, member.TotalCount, pageNumber, pageSize);
        }


        public async Task<MemberDTO> GetMembersByEmailAddress(string emailAddress)
        {
            var member = await _memberRepository.GetMembersByEmailAddress(emailAddress);
            if (member == null)
            {
                _logger.LogWarning("No member found.");
                return null;
            }
            var items = _mapper.Map<List<MemberDTO>>(member);
            var memberDto = _mapper.Map<MemberDTO>(member);
            return memberDto;
        }

        public async Task<IEnumerable<MemberDTO>> GetMembers()
        {
            var member = await _memberRepository.GetMembers();
            if (member == null)
            {
                _logger.LogWarning("No member found.");
                return null;
            }
            var items = _mapper.Map<List<MemberDTO>>(member);
            var memberDto = _mapper.Map<IEnumerable<MemberDTO>>(member);
            return memberDto;
        }
    }
}

