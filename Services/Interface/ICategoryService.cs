using BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface ICategoryService
    {
        Task<bool> AddCategory(Category category);
        Task<bool> UpdateCategory(Category category);
        Task<bool> DeleteCategory(int categoryId);
        Task<Category> GetCategory(int categoryId);
        Task<IEnumerable<Category>> GetCategories();
    }
}
