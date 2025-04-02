using AutoMapper;
using BusinessObject.Entities;
using Core;
using DataAccess.Interface;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Services.Interface;
using Services.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implement
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ICategoryRepository categoryRepository, IProductRepository productRepository, IHubContext<ChatHub> hubContext, IMapper mapper, ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _hubContext = hubContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> AddCategory(CategoryDTO categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            bool isSuccess = await _categoryRepository.AddCategoryAsync(category);
            await _hubContext.Clients.All.SendAsync("ReceiveMessage");
            return isSuccess;
        }

        public async Task<bool> DeleteCategory(int categoryId)
        {
            if (await _productRepository.ExistProductByCategoryId(categoryId))
            {
                throw new Exception("Category is referenced in Product");
            }

            // Get the category before deleting it
            Category category = await _categoryRepository.GetCategory(categoryId);

            bool isSuccess = await _categoryRepository.DeleteCategory(categoryId);
            if (isSuccess)
            {
                await _hubContext.Clients.All.SendAsync("ReceiveMessage");
            }
            return isSuccess;
        }

        public async Task<bool> UpdateCategory(CategoryDTO categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            bool isSuccess = await _categoryRepository.UpdateCategory(category);
            if (isSuccess)
            {
                await _hubContext.Clients.All.SendAsync("ReceiveMessage");
            }
            return isSuccess;
        }

        public async Task<PaginatedList<CategoryDTO>> GetCategories(string name, int pageNumber, int pageSize)
        {
            var categories = await _categoryRepository.GetCategories(name, pageNumber, pageSize);
            if (categories == null)
            {
                _logger.LogWarning("No categories found for the given parameters.");
                return null;
            }
            var items = _mapper.Map<List<CategoryDTO>>(categories.Items);
            return new PaginatedList<CategoryDTO>(items, categories.TotalCount, pageNumber, pageSize);

        }

        public async Task<IEnumerable<CategoryDTO>> GetCategories()
        {
            var categories = await _categoryRepository.GetCategories();
            if (categories == null)
            {
                _logger.LogWarning("No categories found.");
                return null;
            }
            var items = _mapper.Map<List<CategoryDTO>>(categories);
            var categoryDtos = _mapper.Map<IEnumerable<CategoryDTO>>(categories);
            return categoryDtos;
        }

        public async Task<CategoryDTO> GetCategory(int categoryId)
        {
            var category = await _categoryRepository.GetCategory(categoryId);
            if (category == null)
            {
                _logger.LogWarning($"Category with ID {categoryId} not found.");
                return null;
            }
            var categoryDto = _mapper.Map<CategoryDTO>(category);
            return categoryDto;
        }
    }
}
