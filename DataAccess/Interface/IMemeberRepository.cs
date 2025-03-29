using BusinessObject.Entities;
namespace DataAccess.Interface
{
    public interface IMemeberRepository
    {
        Task<bool> AddMemberAsync(Member member);
        Task<bool> UpdateMember(Member member);
        Task<bool> DeleteMember(int memberId);
        Task<Member> GetMember(int memberId);
        Task<IEnumerable<Member>> GetMembers();
        Task<Member?> Login(string email, string password);
        Task<Member> GetMembersByEmailAddress(string emailAddress);

    }
}
