//using BusinessObject.Entities;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Services.Interface
//{
//    public interface IProductService
//    {
//        Task<IEnumerable<Product>> GetAllProductsAsync();
//        Task<(IEnumerable<Product> Products, int TotalCount)> GetAllProductsPageAsync(int pageNumber, int pageSize); 
//        Task<Product> GetProductByIdAsync(int id);
//        Task<Product> CreateProductAsync(Product product);
//        Task<Product> UpdateProductAsync(Product product);
//        Task<bool> DeleteProductAsync(int id);
//        Task<bool> CheckProductInOrderDetailsAsync(int id);
//        Task<(IEnumerable<Product> Products, int TotalCount)> FilterProductsAsync(int pageNumber, int pageSize, string searchName, string searchPriceText, int? categoryId); // Cập nhật để hỗ trợ phân trang
//    }
//}




using Services.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAllProductsAsync();
        Task<(IEnumerable<ProductDTO> Products, int TotalCount)> GetAllProductsPageAsync(int pageNumber, int pageSize);
        Task<ProductDTO> GetProductByIdAsync(int id);
        Task<ProductDTO> CreateProductAsync(ProductDTO productDto);
        Task<ProductDTO> UpdateProductAsync(ProductDTO productDto);
        Task<bool> DeleteProductAsync(int id);
        Task<bool> CheckProductInOrderDetailsAsync(int id);
        Task<(IEnumerable<ProductDTO> Products, int TotalCount)> FilterProductsAsync(int pageNumber, int pageSize, string searchName, string searchPriceText, int? categoryId);
    }
}