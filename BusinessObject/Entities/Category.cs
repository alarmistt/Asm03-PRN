using System.ComponentModel.DataAnnotations;

namespace BusinessObject.Entities
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required, StringLength(40)]
        public string CategoryName { get; set; }

        public string Description { get; set; }

        public ICollection<Product> Products { get; set; }

        public DateTimeOffset? DeletedDate { get; set; } = null;
    }
}
