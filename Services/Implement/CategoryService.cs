using BusinessObject.Entities;
using DataAccess.Interface;
using Services.Interface;

namespace Services.Implement
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<bool> AddCategory(Category category)
        {
            
            return await _categoryRepository.AddCategoryAsync(category);
        }

        public async Task<bool> DeleteCategory(int categoryId)
        {
            
            return await _categoryRepository.DeleteCategory(categoryId);
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
