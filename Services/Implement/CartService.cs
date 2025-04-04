using BusinessObject.Entities;
using Services.Interface;
using Services.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implement
{
    public class CartService : ICartService
    {
        private readonly ICacheService _cacheService;
        private readonly TimeSpan _cartExpiration = TimeSpan.FromMinutes(15); 
        public CartService(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        private string GetCartKey(string userId) => $"cart:{userId}";
        private string GetOrderKey(string userId) => $"order:{userId}";

        public async Task<ShoppingCartDTO> GetCartAsync(string userId)
        {
            return await _cacheService.GetCacheObjectAsync<ShoppingCartDTO>(GetCartKey(userId)) ?? new ShoppingCartDTO();
        }

        public async Task AddToCartAsync(string userId, CartItemDTO item)
        {
            var cart = await GetCartAsync(userId);

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductDTO.ProductId == item.ProductDTO.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                cart.Items.Add(item);
            }
            Console.WriteLine("Add Product to cart");
            await _cacheService.SetCacheResponseAsync(GetCartKey(userId), cart, _cartExpiration);
        }

        public async Task RemoveFromCartAsync(string userId, int productId)
        {
            var cart = await GetCartAsync(userId);

            cart.Items.RemoveAll(i => i.ProductDTO.ProductId == productId);
            Console.WriteLine("Remove Product from cart");

            await _cacheService.SetCacheResponseAsync(GetCartKey(userId), cart, _cartExpiration);
        }

        public async Task ClearCartAsync(string userId)
        {
            Console.WriteLine("Clear cart");
            await _cacheService.RemoveCacheResponseAsync(GetCartKey(userId));
        }

        public async Task UpdateCartItemQuantityAsync(string userId, int productId, int quantityChange)
        {
            var cart = await GetCartAsync(userId);

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductDTO.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantityChange;

                // Nếu số lượng <= 0, xóa sản phẩm khỏi giỏ hàng
                if (existingItem.Quantity <= 0)
                {
                    cart.Items.Remove(existingItem);
                }

                Console.WriteLine($"Updated quantity of product {productId} to {existingItem.Quantity}");

                // Cập nhật lại Redis cache
                await _cacheService.SetCacheResponseAsync(GetCartKey(userId), cart, _cartExpiration);
            }
        }

        public async Task CreateOrderFromCartAsync(string userId)
        {
            var cart = await GetCartAsync(userId);
            if (cart == null || !cart.Items.Any())
            {
                throw new InvalidOperationException("Giỏ hàng trống, không thể tạo đơn hàng.");
            }

            List<OrderDetail> TempOrderDetails = cart.Items.Select(i => new OrderDetail
            {
                ProductId = i.ProductDTO.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.ProductDTO.UnitPrice * i.Quantity  // Tính toán đơn giá
            }).ToList();

            var order = new Order
            {
                MemberId = int.Parse(userId.Trim()),
                OrderDetails = TempOrderDetails,
                OrderDate = DateTime.Now,

            };


            // Lưu đơn hàng vào Redis (tạm thời)
            await _cacheService.SetCacheResponseAsync(GetOrderKey(userId), order, TimeSpan.FromDays(1)); // Lưu đơn hàng trong 1 ngày
            Console.WriteLine("Create Order in redis cart success");
            // Xóa giỏ hàng sau khi tạo đơn
            //await _cacheService.RemoveCacheResponseAsync(GetCartKey(userId));
        }
    }
}
