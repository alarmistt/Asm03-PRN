using System.ComponentModel.DataAnnotations;

namespace BusinessObject.Entities
{
    public class Member
    {
        [Key]
        public int MemberId { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, StringLength(40)]
        public string CompanyName { get; set; }

        [Required, StringLength(15)]
        public string City { get; set; }

        [Required, StringLength(15)]
        public string Country { get; set; }

        [Required, StringLength(30), MinLength(6)]
        public string Password { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}
