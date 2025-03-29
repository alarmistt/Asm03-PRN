using BusinessObject.Base;
using BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Interface
{
    public interface IMemeberManagementDA
    {
        bool AddMember(Member member);
        bool UpdateMember(Member member);
        bool DeleteMember(int memberId);
        Member GetMember(int memberId);
        List<Member> GetMembers();

        List<Member> GetMembersByEmailAddress(string emailAddress);



    }
}
