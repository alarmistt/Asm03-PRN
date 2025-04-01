using BusinessObject.Entities;
using Core;
using DataAccess.Interface;
using Microsoft.AspNetCore.SignalR;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implement
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;

        private readonly IHubContext<ChatHub> _hubContext;

        public CategoryService(ICategoryRepository categoryRepository, IProductRepository productRepository, IHubContext<ChatHub> hubContext)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _hubContext = hubContext;
        }
        public async Task<bool> AddCategory(Category category)
        {
            bool isSuccess = await _categoryRepository.AddCategoryAsync(category);
            await _hubContext.Clients.All.SendAsync("ReceiveMessage");
            return isSuccess;
        }

        public async Task<bool> DeleteCategory(int categoryId)
        {
            if (await _productRepository.ExistProductByCategoryId(categoryId))
            {
                throw new Exception("Category is referenced in Product");
            }

            // Get the category before deleting it
            Category category = await _categoryRepository.GetCategory(categoryId);

            bool isSuccess = await _categoryRepository.DeleteCategory(categoryId);
            if (isSuccess)
            {
                await _hubContext.Clients.All.SendAsync("ReceiveMessage");
            }
            return isSuccess;
        }

        public async Task<bool> UpdateCategory(Category category)
        {
            bool isSuccess = await _categoryRepository.UpdateCategory(category);
            if (isSuccess)
            {
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Category updated", category);
            }
            return isSuccess;
        }

        public async Task<PaginatedList<Category>> GetCategories(string name, int pageNumber, int pageSize)
        {   
            return await _categoryRepository.GetCategories(name, pageNumber, pageSize);
        }

        public async Task<IEnumerable<Category>> GetCategories()
        {
            return await _categoryRepository.GetCategories();
        }

        public async Task<Category> GetCategory(int categoryId)
        {
            return await _categoryRepository.GetCategory(categoryId);
            
        }

        
    }
}
