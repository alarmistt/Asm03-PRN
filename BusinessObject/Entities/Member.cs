
using System.ComponentModel.DataAnnotations;

namespace BusinessObject.Entities
{
    public class Member
    {
        [Key]
        public int MemberId { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string CompanyName { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Country { get; set; }
        [Required]
        public string Role { get; set; } = "User";

        [Required]
        public string Password { get; set; }
        public DateTimeOffset? DeletedDate { get; set; } = null;

        public ICollection<Order> Orders { get; set; }
    }
}
