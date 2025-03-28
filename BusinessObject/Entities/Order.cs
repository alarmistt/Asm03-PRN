using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace  BusinessObject.Entities
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [ForeignKey("Member")]
        public int MemberId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public DateTime RequiredDate { get; set; }

        public DateTime? ShippedDate { get; set; }

        [Column(TypeName = "money"), Range(0, double.MaxValue)]
        public decimal Freight { get; set; }

        public Member Member { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }

}
