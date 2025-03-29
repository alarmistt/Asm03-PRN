using DataAccess.Interface;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implement
{
    internal class MemberService : IMemberService
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
