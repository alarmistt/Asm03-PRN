using BusinessObject.Entities;
using Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.Interface
{
    public interface IMemberRepository
    {
        Task<bool> AddMember(Member member);
        Task<bool> UpdateMember(Member member);
        Task<bool> DeleteMember(int memberId);
        Task<Member> GetMember(int memberId);

        Task<IEnumerable<Member>> GetMembers();

        Task<PaginatedList<Member>> GetMembers(int pageNumber, int pageSize);
        Task<PaginatedList<Member>> GetMembers(string email, string companyName, string country, int pageNumber, int pageSize);
        Task<Member?> Login(string email, string password);
        Task<Member> GetMembersByEmailAddress(string emailAddress);
    }
}

