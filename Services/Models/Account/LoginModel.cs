using BusinessObject.Entities;
using System.ComponentModel.DataAnnotations;

namespace Services.Models.Account
{
    public class LoginModel
    {
        public Member Account { get; set; }

        public string Token { get; set; }
    }
}
