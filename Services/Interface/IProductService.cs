using BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<(IEnumerable<Product> Products, int TotalCount)> GetAllProductsPageAsync(int pageNumber, int pageSize); 
        Task<Product> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(Product product);
        Task<Product> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);
        Task<bool> CheckProductInOrderDetailsAsync(int id);
        Task<(IEnumerable<Product> Products, int TotalCount)> FilterProductsAsync(int pageNumber, int pageSize, string searchName, string searchPriceText, int? categoryId); // Cập nhật để hỗ trợ phân trang
    }
}
