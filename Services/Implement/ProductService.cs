using BusinessObject.Entities;
using DataAccess.Interface;
using Microsoft.AspNetCore.SignalR;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataAccess.Implement;

namespace Services.Implement
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            return await _repository.AddAsync(product);
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            return await _repository.UpdateAsync(product);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
        public async Task<bool> CheckProductInOrderDetailsAsync(int id)
        {
            return await _repository.IsProductInOrderDetailsAsync(id);
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string name, decimal? unitPrice)
        {
            return await _repository.SearchAsync(name, unitPrice);
        }
    }
}
