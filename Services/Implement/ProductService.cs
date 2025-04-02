
using BusinessObject.Entities;
using DataAccess.Interface;
using Microsoft.AspNetCore.SignalR;
using Services.Interface;

namespace Services.Implement
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly IHubContext<ChatHub> _hubContext;

        public ProductService(IProductRepository repository, IHubContext<ChatHub> hubContext)
        {
            _repository = repository;
            _hubContext = hubContext;
        }
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _repository.GetAllAsync();
        }
        public async Task<(IEnumerable<Product> Products, int TotalCount)> GetAllProductsPageAsync(int pageNumber, int pageSize)
        {
            return await _repository.GetAllPageAsync(pageNumber, pageSize);
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            var result = await _repository.AddAsync(product);
            await _hubContext.Clients.All.SendAsync("ReceiveMessage");
            return result;
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            var result = await _repository.UpdateAsync(product);
            await _hubContext.Clients.All.SendAsync("ReceiveMessage");
            return result;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var result = await _repository.DeleteAsync(id);
            await _hubContext.Clients.All.SendAsync("ReceiveMessage");
            return result;
        }

        public async Task<bool> CheckProductInOrderDetailsAsync(int id)
        {
            return await _repository.IsProductInOrderDetailsAsync(id);
        }

        public async Task<(IEnumerable<Product> Products, int TotalCount)> FilterProductsAsync(int pageNumber, int pageSize, string searchName, string searchPriceText, int? categoryId)
        {
            return await _repository.FilterProductsAsync(pageNumber, pageSize, searchName, searchPriceText, categoryId);
        }
    }
}