using BusinessObject.Entities;
using DataAccess.Interface;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implement
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IOrderDetailRepository _orderDetailRepository;

        public OrderDetailService(IOrderDetailRepository orderDetailRepository)
        {
            _orderDetailRepository = orderDetailRepository;
        }

        public async Task<IEnumerable<OrderDetail>> GetAllAsync()
        {
            return await _orderDetailRepository.GetAllAsync();
        }

        public async Task<OrderDetail?> GetByIdAsync(int orderId, int productId)
        {
            return await _orderDetailRepository.GetByIdAsync(orderId, productId);
        }

        public async Task AddAsync(OrderDetail orderDetail)
        {
            // Business logic có thể được thêm vào đây nếu cần
            await _orderDetailRepository.AddAsync(orderDetail);
        }

        public async Task UpdateAsync(OrderDetail orderDetail)
        {
            // Business logic có thể được thêm vào đây nếu cần
            await _orderDetailRepository.UpdateAsync(orderDetail);
        }

        public async Task DeleteAsync(int orderId, int productId)
        {
            // Business logic có thể được thêm vào đây nếu cần
            await _orderDetailRepository.DeleteAsync(orderId, productId);
        }
    }
}
