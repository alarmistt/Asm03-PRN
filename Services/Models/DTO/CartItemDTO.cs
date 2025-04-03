using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models.DTO
{
    public class CartItemDTO
    {
        public ProductDTO ProductDTO { get; set; }
        public int Quantity { get; set; } // Quantity of the product in the cart
    }
}
