using BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Interface
{
    public interface IMemberManager
{
        private readonly EStoreContext _context;

        public MemberManager(EStoreContext context)
        {
            _context = context;
        }

        public async Task<Member> GetMemberByEmail(string email)
        {
            return await _context.Member.FirstOrDefaultAsync(m => m.Email == email);
        }

        public async Task<Member> GetMemberById(int id)
        {
            return await _context.Member.FindAsync(id);
        }

        public async Task<Member> AddMember(Member member)
        {
            _context.Member.Add(member);
            await _context.SaveChangesAsync();
            return member;
        }

        public async Task<Member> UpdateMember(Member member)
        {
            _context.Member.Update(member);
            await _context.SaveChangesAsync();
            return member;
        }

        public async Task<Member> DeleteMember(int id)
        {
            var member = await _context.Member.FindAsync(id);
            if (member != null)
            {
                _context.Member.Remove(member);
                await _context.SaveChangesAsync();
            }
            return member;
        }


    }
}
