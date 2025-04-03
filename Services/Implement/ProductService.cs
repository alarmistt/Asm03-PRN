
using AutoMapper;
using BusinessObject.Entities;
using DataAccess.Interface;
using Microsoft.AspNetCore.SignalR;
using Services.Interface;
using Services.Models.DTO;

namespace Services.Implement
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository repository, IHubContext<ChatHub> hubContext, IMapper mapper)
        {
            _repository = repository;
            _hubContext = hubContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
        {
            var products = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task<(IEnumerable<ProductDTO> Products, int TotalCount)> GetAllProductsPageAsync(int pageNumber, int pageSize)
        {
            var (products, totalCount) = await _repository.GetAllPageAsync(pageNumber, pageSize);
            var productDtos = _mapper.Map<IEnumerable<ProductDTO>>(products);
            return (productDtos, totalCount);
        }

        public async Task<ProductDTO> GetProductByIdAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<ProductDTO> CreateProductAsync(ProductDTO productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            var result = await _repository.AddAsync(product);
            await _hubContext.Clients.All.SendAsync("ReceiveMessage");
            return _mapper.Map<ProductDTO>(result);
        }

        public async Task<ProductDTO> UpdateProductAsync(ProductDTO productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            var result = await _repository.UpdateAsync(product);
            await _hubContext.Clients.All.SendAsync("ReceiveMessage");
            return _mapper.Map<ProductDTO>(result);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var result = await _repository.DeleteAsync(id);
            await _hubContext.Clients.All.SendAsync("ReceiveMessage");
            return result;
        }

        public async Task<bool> CheckProductInOrderDetailsAsync(int id)
        {
            return await _repository.IsProductInOrderDetailsAsync(id);
        }

        public async Task<(IEnumerable<ProductDTO> Products, int TotalCount)> FilterProductsAsync(int pageNumber, int pageSize, string searchName, string searchPriceText, int? categoryId)
        {
            var (products, totalCount) = await _repository.FilterProductsAsync(pageNumber, pageSize, searchName, searchPriceText, categoryId);
            var productDtos = _mapper.Map<IEnumerable<ProductDTO>>(products);
            return (productDtos, totalCount);
        }
    }
}