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

        public Task<bool> Login(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}
