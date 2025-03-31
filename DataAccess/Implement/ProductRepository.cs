using BusinessObject.Base;
using BusinessObject.Entities;
using DataAccess.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

            // Cập nhật từng field cụ thể
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.ProductName = product.ProductName;
            existingProduct.Weight = product.Weight;
            existingProduct.UnitPrice = product.UnitPrice;
            existingProduct.UnitsInStock = product.UnitsInStock;

            Console.WriteLine($"Updated: CategoryId={existingProduct.CategoryId}, Name={existingProduct.ProductName}, " +
                             $"Weight={existingProduct.Weight}, UnitPrice={existingProduct.UnitPrice}, " +
                             $"UnitsInStock={existingProduct.UnitsInStock}");

            var rowsAffected = await _context.SaveChangesAsync();
            // Bỏ kiểm tra rowsAffected hoặc ghi log thay vì ném lỗi
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

            // Kiểm tra xem sản phẩm có trong OrderDetail hay không
            var isProductInOrder = await IsProductInOrderDetailsAsync(id);
            if (isProductInOrder)
            {
                // Không xóa và trả về false nếu sản phẩm tồn tại trong OrderDetail
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

        public async Task<IEnumerable<Product>> SearchAsync(string name, decimal? unitPrice)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(p => p.ProductName.Contains(name));

            if (unitPrice.HasValue)
                query = query.Where(p => p.UnitPrice == unitPrice.Value);

            return await query.ToListAsync();
        }
    }
}