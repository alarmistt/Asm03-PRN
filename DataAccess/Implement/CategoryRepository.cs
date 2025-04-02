using BusinessObject.Entities;
using Core;
using DataAccess.Base;
using DataAccess.Interface;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Implement
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly EStoreContext _context;

        public CategoryRepository(EStoreContext context)
        {
            _context = context;
        }

        public async Task<bool> AddCategoryAsync(Category category)
        {
            var exist = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryName == category.CategoryName);

            if (exist != null)
            {
                throw new Exception("Category already exists");
            }
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCategory(int categoryId)
        {
            var exist = await _context.Categories.FindAsync(categoryId);
            if (exist != null)
            {
                _context.Categories.Remove(exist);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<PaginatedList<Category>> GetCategories(string name, int pageNumber, int pageSize)
        {
            var query = _context.Categories.AsQueryable();
            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(x => x.CategoryName.ToLower().Contains(name.ToLower()));
            }
            query = query.OrderByDescending(x => x.CategoryId);
            return await PaginatedList<Category>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<PaginatedList<Category>> GetCategories(int pageNumber, int pageSize)
        {
            var query = _context.Categories.AsQueryable();
            query = query.OrderByDescending(x => x.CategoryId);
            return await PaginatedList<Category>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<IEnumerable<Category>> GetCategories(string name = "")
        {
            var query = _context.Categories.AsQueryable();
            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(x => x.CategoryName.ToLower().Contains(name.ToLower()));
            }
            query = query.OrderByDescending(x => x.CategoryId);
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> GetCategory(int categoryId)
        {
            return await _context.Categories.FindAsync(categoryId);
        }

        public async Task<bool> UpdateCategory(Category category)
        {
            var exist = await _context.Categories.FindAsync(category.CategoryId);

            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(category);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
