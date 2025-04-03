using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models.DTO
{
    public class ShoppingCartDTO
    {
        public List<CartItemDTO> Items { get; set; } = new List<CartItemDTO>();


    }
}
