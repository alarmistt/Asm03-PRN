using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BusinessObject.Entities
{
    public class OrderDetail
    {
        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [Column(TypeName = "money"), Required, Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required, Range(0, 1)]
        public float Discount { get; set; }
        public DateTimeOffset? DeletedDate { get; set; } = null;
        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}
