using BusinessObject.Entities;
using Core;
using DataAccess.Base;
using DataAccess.Interface;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Implement
{
    public class MemberRepository : IMemberRepository
    {
        private readonly IDbContextFactory<EStoreContext> _contextFactory;

        public MemberRepository(IDbContextFactory<EStoreContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<bool> AddMember(Member member)
        {
            using var context = _contextFactory.CreateDbContext();
            var exist = await context.Member.FirstOrDefaultAsync(m => m.Email == member.Email);

            if (exist != null)
            {
                throw new Exception("Email already exists");
            }
            context.Member.Add(member);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteMember(int memberId)
        {
            using var context = _contextFactory.CreateDbContext();
            var exist = await context.Member.FindAsync(memberId);
            if (exist != null)
            {
                context.Member.Remove(exist);
                await context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<Member?> GetMember(int memberId)
        {
            using var context = _contextFactory.CreateDbContext();
            var member = await context.Member.FindAsync(memberId);

            if (member != null)
            {
                return member;
            }
            else
            {
                return null;
            }
        }

        public async Task<PaginatedList<Member>> GetMembers(int pageNumber, int pageSize)
        {
            using var context = _contextFactory.CreateDbContext();
            var query = context.Member.AsQueryable();
            query = query.OrderByDescending(x => x.MemberId);
            return await PaginatedList<Member>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<PaginatedList<Member>> GetMembers(string email, string companyName, string country, int pageNumber, int pageSize)
        {
            using var context = _contextFactory.CreateDbContext();
            var query = context.Member.AsQueryable();
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
            query = query.OrderByDescending(x => x.MemberId);
            return await PaginatedList<Member>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<IEnumerable<Member>> GetMembers()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Member.ToListAsync();
        }

        public async Task<Member?> GetMembersByEmailAddress(string emailAddress)
        {
            using var context = _contextFactory.CreateDbContext();
            var exist = await context.Member.FirstOrDefaultAsync(m => m.Email == emailAddress);

            if (exist != null)
            {
                return exist;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> UpdateMember(Member member)
        {
            using var context = _contextFactory.CreateDbContext();
            var existingMember = await context.Member.FindAsync(member.MemberId);
            if (existingMember != null)
            {
                context.Entry(existingMember).State = EntityState.Detached;
            }
            context.Member.Update(member);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
