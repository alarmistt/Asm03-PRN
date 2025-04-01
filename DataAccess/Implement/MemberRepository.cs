using BusinessObject.Base;
using BusinessObject.Entities;
using DataAccess.Interface;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Implement
{
    public class MemberRepository : IMemeberRepository
    {
        private readonly EStoreContext _context;

        public MemberRepository(EStoreContext context)
        {
            _context = context;
        }

        public async Task<bool> AddMember(Member member)
        {
            var exist = await _context.Member.FirstOrDefaultAsync(m => m.Email == member.Email);

            if (exist != null)
            {
                throw new Exception("Email already exists");
            }
            _context.Member.Add(member);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteMember(int memberId)
        {
            var exist = await _context.Member.FindAsync(memberId);
            if (exist != null)
            {
                _context.Member.Remove(exist);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }

        }

        public async Task<Member> GetMember(int memberId)
        {
            var member = await _context.Member.FindAsync(memberId);

            if (member != null)
            {
                return member;
            }
            else
            {
                return null;
            }
        }

        public async Task<IEnumerable<Member>> GetMembers()
        {
            var members = await _context.Member.ToListAsync();
            return members;
        }

        public async Task<IEnumerable<Member>> GetMembers(string email = "", string companyName = "", string country = "")
        {
            var query = _context.Member.AsQueryable();
            if (!string.IsNullOrWhiteSpace(email))
            {
                query = query.Where(x => x.Email.ToLower().Contains(email.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(companyName))
            {
                query = query.Where(x => x.CompanyName.ToLower().Contains(companyName.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(country))
            {
                query = query.Where(x => x.Country.ToLower().Contains(country.ToLower()));
            }
            return await query.ToListAsync();
        }

        public async Task<Member> GetMembersByEmailAddress(string emailAddress)
        {
            var exist = await _context.Member.FirstOrDefaultAsync(m => m.Email == emailAddress);

            if (exist != null)
            {
                return exist;
            }
            else
            {
                return null;
            }
        }

        public async Task<Member?> Login(string email, string password)
        {
            return await _context.Member.FirstOrDefaultAsync(x => x.Email == email && x.Password == password);
        }

        public async Task<bool> UpdateMember(Member member)
        {
            var exist = _context.Member.FindAsync(member.MemberId);

            if (exist != null)
            {
                _context.Member.Update(member);
                _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
