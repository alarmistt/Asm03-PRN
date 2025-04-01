using BusinessObject.Entities;
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

        public CategoryService(ICategoryRepository categoryRepository, IProductRepository productRepository)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
        }
        public async Task<bool> AddCategory(Category category)
        {
            
            return await _categoryRepository.AddCategoryAsync(category);
        }

        public async Task<bool> DeleteCategory(int categoryId)
        {
            if (await _productRepository.ExistProductByCategoryId(categoryId))
            {
                throw new Exception("Category is referenced in Product");
            }
            return await _categoryRepository.DeleteCategory(categoryId);
        }

        public async Task<IEnumerable<Category>> GetCategories(string name = "")
        {   
            return await _categoryRepository.GetCategories(name);
        }

        public async Task<IEnumerable<Category>> GetCategories()
        {
            return await _categoryRepository.GetCategories();
        }

        public async Task<Category> GetCategory(int categoryId)
        {
            return await _categoryRepository.GetCategory(categoryId);
            
        }

        public async Task<bool> UpdateCategory(Category category)
        {
           
            return await _categoryRepository.UpdateCategory(category);
        }
    }
}
