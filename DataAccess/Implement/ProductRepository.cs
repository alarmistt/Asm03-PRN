
using BusinessObject.Entities;
using DataAccess.Base;
using DataAccess.Interface;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Implement
{
    public class ProductRepository : IProductRepository
    {
        private readonly EStoreContext _context;

        public ProductRepository(EStoreContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<(IEnumerable<Product> Products, int TotalCount)> GetAllPageAsync(int pageNumber, int pageSize)
        {
            int totalCount = await _context.Products.CountAsync();
            var products = await _context.Products
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (products, totalCount);
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product> AddAsync(Product product)
        {
            Console.WriteLine($"Saving to DB: CategoryId={product.CategoryId}, Name={product.ProductName}, Weight={product.Weight}, " +
                             $"UnitPrice={product.UnitPrice}, UnitsInStock={product.UnitsInStock}");
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            var existingProduct = await _context.Products.FindAsync(product.ProductId);
            if (existingProduct == null)
            {
                throw new Exception($"Không tìm thấy sản phẩm với ID {product.ProductId}");
            }

            Console.WriteLine($"Existing: ProductId={existingProduct.ProductId}, CategoryId={existingProduct.CategoryId}, " +
                             $"Name={existingProduct.ProductName}, Weight={existingProduct.Weight}, " +
                             $"UnitPrice={existingProduct.UnitPrice}, UnitsInStock={existingProduct.UnitsInStock}");
            Console.WriteLine($"New: ProductId={product.ProductId}, CategoryId={product.CategoryId}, " +
                             $"Name={product.ProductName}, Weight={product.Weight}, " +
                             $"UnitPrice={product.UnitPrice}, UnitsInStock={product.UnitsInStock}");

            existingProduct.CategoryId = product.CategoryId;
            existingProduct.ProductName = product.ProductName;
            existingProduct.Weight = product.Weight;
            existingProduct.UnitPrice = product.UnitPrice;
            existingProduct.UnitsInStock = product.UnitsInStock;

            Console.WriteLine($"Updated: CategoryId={existingProduct.CategoryId}, Name={existingProduct.ProductName}, " +
                             $"Weight={existingProduct.Weight}, UnitPrice={existingProduct.UnitPrice}, " +
                             $"UnitsInStock={existingProduct.UnitsInStock}");

            var rowsAffected = await _context.SaveChangesAsync();
            if (rowsAffected == 0)
            {
                Console.WriteLine("Không có thay đổi nào được lưu vào cơ sở dữ liệu.");
            }

            return existingProduct;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return false;
            }

            var isProductInOrder = await IsProductInOrderDetailsAsync(id);
            if (isProductInOrder)
            {
                Console.WriteLine($"Product with ID {id} cannot be deleted because it is referenced in OrderDetail.");
                return false;
            }

            _context.Products.Remove(product);
            var rowsAffected = await _context.SaveChangesAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> IsProductInOrderDetailsAsync(int id)
        {
            return await _context.OrderDetail.AnyAsync(od => od.ProductId == id);
        }

        public Task<bool> ExistProductByCategoryId(int categoryId)
        {
            bool isExist = _context.Products.Any(p => p.CategoryId == categoryId);
            return Task.FromResult(isExist);
        }

        public async Task<(IEnumerable<Product> Products, int TotalCount)> FilterProductsAsync(int pageNumber, int pageSize, string searchName, string searchPriceText, int? categoryId)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(searchName))
            {
                query = query.Where(p => p.ProductName.Contains(searchName));
            }

            if (!string.IsNullOrEmpty(searchPriceText))
            {
                query = query.Where(p => p.UnitPrice.ToString().Contains(searchPriceText));
            }

            if (categoryId.HasValue && categoryId.Value != 0)
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