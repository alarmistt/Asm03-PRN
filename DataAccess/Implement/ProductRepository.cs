
using BusinessObject.Entities;
using DataAccess.Base;
using DataAccess.Interface;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Implement
{
    public class ProductRepository : IProductRepository
    {
        private readonly IDbContextFactory<EStoreContext> _contextFactory;

        public ProductRepository(IDbContextFactory<EStoreContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Products
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Product> Products, int TotalCount)> GetAllPageAsync(int pageNumber, int pageSize)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            int totalCount = await context.Products.CountAsync();
            var products = await context.Products
                .Include(p => p.Category)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (products, totalCount);
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<Product> AddAsync(Product product)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.Products.Add(product);
            await context.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> UpdateAsync(Product product)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var existingProduct = await context.Products.FindAsync(product.ProductId);
            if (existingProduct == null)
            {
                return null;
            }

            // Cập nhật dữ liệu
            context.Entry(existingProduct).CurrentValues.SetValues(product);
            await context.SaveChangesAsync();
            return existingProduct;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var product = await context.Products.FindAsync(id);
            if (product == null)
            {
                return false;
            }

            // Kiểm tra xem sản phẩm có được sử dụng trong OrderDetail không
            if (await context.OrderDetail.AnyAsync(od => od.ProductId == id))
            {
                Console.WriteLine($"Product with ID {id} cannot be deleted because it is referenced in OrderDetail.");
                return false;
            }

            context.Products.Remove(product);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ExistProductByCategoryId(int categoryId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Products.AnyAsync(p => p.CategoryId == categoryId);
        }

        public async Task<bool> IsProductInOrderDetailsAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.OrderDetail.AnyAsync(od => od.ProductId == id);
        }


        public async Task<(IEnumerable<Product> Products, int TotalCount)> FilterProductsAsync(int pageNumber, int pageSize, string searchName, string searchPriceText, int? categoryId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var query = context.Products
                .Include(p => p.Category) // Ensure Category is included
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchName))
            {
                query = query.Where(p => p.ProductName.Contains(searchName));
            }

            if (!string.IsNullOrEmpty(searchPriceText))
            {
                // Convert UnitPrice to string and check if it contains the searchPriceText
                query = query.Where(p => p.UnitPrice.ToString().Contains(searchPriceText));
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            int totalCount = await query.CountAsync();
            var products = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (products, totalCount);
        }
    }

}