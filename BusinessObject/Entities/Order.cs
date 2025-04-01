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


        /// <summary>
        /// Tính tổng giá trị đơn hàng: phí vận chuyển cộng với tổng giá trị các OrderDetail.
        /// Công thức: Total = Freight + Σ (UnitPrice * Quantity * (1 - Discount))
        /// </summary>
        public decimal CalculateTotalAmount()
        {
            decimal total = Freight;

            if (OrderDetails != null && OrderDetails.Any())
            {
                foreach (var detail in OrderDetails)
                {
                    total += detail.UnitPrice *  (1 - (decimal)detail.Discount);
                }
            }

            return total;
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ của đơn hàng và các chi tiết đơn hàng.
        /// </summary>
        public void ValidateOrder()
        {
            if (RequiredDate < OrderDate)
                throw new InvalidOperationException("Required date cannot be earlier than order date.");

            if (ShippedDate.HasValue && ShippedDate < OrderDate)
                throw new InvalidOperationException("Shipped date cannot be earlier than order date.");

            if (Freight < 0)
                throw new InvalidOperationException("Freight cannot be negative.");

            if (OrderDetails == null || !OrderDetails.Any())
                throw new InvalidOperationException("Order must have at least one order detail.");

            foreach (var detail in OrderDetails)
            {
                if (detail.Quantity < 1)
                    throw new InvalidOperationException("Order detail quantity must be at least 1.");

                if (detail.UnitPrice < 0)
                    throw new InvalidOperationException("Order detail unit price cannot be negative.");

                if (detail.Discount < 0 || detail.Discount > 1)
                    throw new InvalidOperationException("Order detail discount must be between 0 and 1.");
            }
        }
    }

}
