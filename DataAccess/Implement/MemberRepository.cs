using BusinessObject.Base;
using BusinessObject.Entities;
using DataAccess.Interface;

namespace DataAccess.Implement
{
    public class MemberRepository : IMemeberRepository
    {
        private readonly EStoreContext _context;

        public MemberRepository(EStoreContext context)
        {
            _context = context;
        }

        public async Task<bool> AddMemberAsync(Member member)
        {
            var exist = await _context.Member.FindAsync(member.Email);

            if (exist != null)
            {
                throw new Exception("Email already exists");
            }
            _context.Member.Add(member);
            await _context.SaveChangesAsync();
            return true;
        }

        public Task<bool> DeleteMember(int memberId)
        {
            throw new NotImplementedException();
        }

        public Task<Member> GetMember(int memberId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Member>> GetMembers()
        {
            throw new NotImplementedException();
        }

        public async Task<Member> GetMembersByEmailAddress(string emailAddress)
        {
            var member = await _context.Member.FindAsync(emailAddress);

            if (member != null)
            {
                return member;
            }
            else
            {
                return null;
            }
        }

        public Task<bool> UpdateMember(Member member)
        {
            throw new NotImplementedException();
        }
    }
}
