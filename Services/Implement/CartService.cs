using Services.Interface;
using Services.Models.DTO;

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

                int newQuantity = existingItem.Quantity + quantityChange;


                if (newQuantity > existingItem.ProductDTO.UnitsInStock)
                {
                    newQuantity = existingItem.ProductDTO.UnitsInStock;
                    Console.WriteLine($"Quantity for product {productId} limited to available stock: {existingItem.ProductDTO.UnitsInStock}");
                }


                if (newQuantity <= 0)
                {
                    cart.Items.Remove(existingItem);
                    Console.WriteLine($"Removed product {productId} from cart as quantity is zero or less.");
                }
                else
                {
                    existingItem.Quantity = newQuantity;
                    Console.WriteLine($"Updated quantity of product {productId} to {existingItem.Quantity}");
                }
                await _cacheService.SetCacheResponseAsync(GetCartKey(userId), cart, _cartExpiration);
            }
        }


    }
}
