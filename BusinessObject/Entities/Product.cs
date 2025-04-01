using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Entities
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        
        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        [Required, StringLength(40)]
        public string ProductName { get; set; }

        [Required, StringLength(20)]
        public string Weight { get; set; }

        [Column(TypeName = "money"), Required, Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Required, Range(0, int.MaxValue)]
        public int UnitsInStock { get; set; }

        public Category Category { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
