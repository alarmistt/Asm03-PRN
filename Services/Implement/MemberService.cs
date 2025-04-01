using BusinessObject.Entities;
using DataAccess.Interface;
using Services.Interface;

namespace Services.Implement
{
    public class MemberService : IMemberService
    {
        private readonly IMemeberRepository _meberRepository;

        public MemberService(IMemeberRepository memeberRepository) 
        {
        
            _meberRepository = memeberRepository;
        }

        public async Task<bool> AddMember(Member member)
        {
            return await _meberRepository.AddMember(member);
        }

        public async Task<bool> DeleteMember(int memberId)
        {
            return await _meberRepository.DeleteMember(memberId);
        }

        public async Task<Member> GetMember(int memberId)
        {
            return await _meberRepository.GetMember(memberId);
        }

        public async Task<IEnumerable<Member>> GetMembers()
        {
            return await _meberRepository.GetMembers();
        }

        public Task<IEnumerable<Member>> GetMembers(string email = "", string companyName = "", string country = "")
        {

            return _meberRepository.GetMembers(email, companyName, country);
        }

        public Task<bool> Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateMember(Member member)
        {
            return await _meberRepository.UpdateMember(member);
        }
    }
}
