using BusinessObject.Entities;
using Core;
using Services.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface ICategoryService
    {
        Task<bool> AddCategory(CategoryDTO category);
        Task<bool> UpdateCategory(CategoryDTO category);
        Task<bool> DeleteCategory(int categoryId);
        Task<CategoryDTO> GetCategory(int categoryId);
        Task<PaginatedList<CategoryDTO>> GetCategories(string name, int pageNumber, int pageSize);
        Task<IEnumerable<CategoryDTO>> GetCategories();
    }


}
