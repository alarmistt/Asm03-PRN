using BusinessObject.Entities;
using Core;
using Services.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IMemberService
    {
        Task<bool> AddMember(MemberDTO member);
        Task<bool> UpdateMember(MemberDTO member);
        Task<bool> DeleteMember(int memberId);
        Task<MemberDTO> GetMember(int memberId);
        Task<PaginatedList<MemberDTO>> GetMembers(int pageNumber, int pageSize);
        Task<PaginatedList<MemberDTO>> GetMembers(string email, string companyName, string country, int pageNumber, int pageSize);

        Task<IEnumerable<MemberDTO>> GetMembers();
        Task<MemberDTO> GetMembersByEmailAddress(string emailAddress);
    }
}

