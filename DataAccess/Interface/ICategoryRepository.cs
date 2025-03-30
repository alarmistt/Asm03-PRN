using BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Interface
{
    public interface ICategoryRepository
    {
        Task<bool> AddCategoryAsync(Category category);
        Task<bool> UpdateCategory(Category category);
        Task<bool> DeleteCategory(int categoryId);
        Task<Category> GetCategory(int categoryId);
        Task<IEnumerable<Category>> GetCategories();
    }
}
