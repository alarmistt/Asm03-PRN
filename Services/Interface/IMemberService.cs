using BusinessObject.Entities;
using Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IMemberService
    {
        Task<bool> AddMember(Member member);
        Task<bool> UpdateMember(Member member);
        Task<bool> DeleteMember(int memberId);
        Task<Member> GetMember(int memberId);
        Task<PaginatedList<Member>> GetMembers(int pageNumber, int pageSize);
        Task<PaginatedList<Member>> GetMembers(string email, string companyName, string country, int pageNumber, int pageSize);

        Task<IEnumerable<Member>> GetMembers();
        Task<Member> GetMembersByEmailAddress(string emailAddress);
    }
}

