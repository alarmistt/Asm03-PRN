using AutoMapper.Execution;
using BusinessObject.Base;
using BusinessObject.Entities;
using DataAccess.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var exist = await _context.Categories.FindAsync(category.CategoryName);

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

        public async Task<IEnumerable<Category>> GetCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category?> GetCategory(int categoryId)
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
