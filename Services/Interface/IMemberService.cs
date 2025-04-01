using BusinessObject.Entities;

namespace Services.Interface
{
    public interface IMemberService
    {
        Task<bool> Login(string username, string password);

        Task<bool> AddMember(Member member);
        Task<bool> UpdateMember(Member member);
        Task<bool> DeleteMember(int memberId);
        Task<Member> GetMember(int memberId);
        Task<IEnumerable<Member>> GetMembers();

        Task<IEnumerable<Member>> GetMembers(string email = "", string companyName = "", string country = "");
    }
}
