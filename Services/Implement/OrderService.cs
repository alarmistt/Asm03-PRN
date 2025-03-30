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
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _orderRepository.GetByIdAsync(orderId);
        }

        public async Task<bool> CreateOrderAsync(Order order)
        {
            return await _orderRepository.AddAsync(order);
        }

        public async Task<bool> UpdateOrderAsync(Order order)
        {
            return await _orderRepository.UpdateAsync(order);
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            return await _orderRepository.DeleteAsync(orderId);
        }
    }
}
