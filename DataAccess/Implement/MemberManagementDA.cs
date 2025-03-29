using BusinessObject.Base;
using BusinessObject.Entities;
using DataAccess.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Implement
{
    public class MemberManagementDA : IMemeberManagementDA
    {
        private readonly EStoreContext _context;

        public MemberManagementDA(EStoreContext context)
        {
            _context = context;
        }
        public bool AddMember(Member member)
        {
            throw new NotImplementedException();

        }

        public bool DeleteMember(int memberId)
        {
            throw new NotImplementedException();
        }

        public Member GetMember(int memberId)
        {
            throw new NotImplementedException();
        }

        public List<Member> GetMembers()
        {
            throw new NotImplementedException();
        }

        public List<Member> GetMembersByEmailAddress(string emailAddress)
        {
            throw new NotImplementedException();
        }

        public bool UpdateMember(Member member)
        {
            throw new NotImplementedException();
        }
    }
}
