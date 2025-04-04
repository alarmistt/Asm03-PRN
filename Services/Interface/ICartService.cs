using BusinessObject.Entities;
using Services.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface ICartService
    {
        Task<ShoppingCartDTO> GetCartAsync(string userId);
        Task AddToCartAsync(string userId, CartItemDTO item);
        Task RemoveFromCartAsync(string userId, int productId);
        Task ClearCartAsync(string userId);

        Task UpdateCartItemQuantityAsync(string userId, int productId, int quantityChange);
    }
}
