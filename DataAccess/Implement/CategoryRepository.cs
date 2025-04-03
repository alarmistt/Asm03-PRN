using BusinessObject.Entities;
using Core;
using DataAccess.Base;
using DataAccess.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Implement
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IDbContextFactory<EStoreContext> _contextFactory;

        public CategoryRepository(IDbContextFactory<EStoreContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<bool> AddCategoryAsync(Category category)
        {
            using var _context = _contextFactory.CreateDbContext();
            var exist = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryName == category.CategoryName);

            if (exist != null)
            {
                throw new Exception("Category already exists");
            }

            _context.Categories.Add(category);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteCategory(int categoryId)
        {
            using var _context = _contextFactory.CreateDbContext();
            var exist = await _context.Categories.FindAsync(categoryId);
            if (exist == null)
            {
                return false;
            }

            _context.Categories.Remove(exist);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<PaginatedList<Category>> GetCategories(string name, int pageNumber, int pageSize)
        {
            using var _context = _contextFactory.CreateDbContext();
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
            using var _context = _contextFactory.CreateDbContext();
            var query = _context.Categories.AsQueryable();
            query = query.OrderByDescending(x => x.CategoryId);
            return await PaginatedList<Category>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<IEnumerable<Category>> GetCategories(string name = "")
        {
            using var _context = _contextFactory.CreateDbContext();
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
            using var _context = _contextFactory.CreateDbContext();
            return await _context.Categories
                .OrderByDescending(x => x.CategoryId)
                .ToListAsync();
        }

        public async Task<Category> GetCategory(int categoryId)
        {
            using var _context = _contextFactory.CreateDbContext();
            return await _context.Categories.FindAsync(categoryId);
        }

        public async Task<bool> UpdateCategory(Category category)
        {
            using var _context = _contextFactory.CreateDbContext();
            var exist = await _context.Categories.FindAsync(category.CategoryId);
            if (exist == null)
            {
                return false;
            }

            _context.Entry(exist).CurrentValues.SetValues(category);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}