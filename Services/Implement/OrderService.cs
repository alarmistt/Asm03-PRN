﻿using BusinessObject.Entities;
using DataAccess.Implement;
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
        private readonly IOrderDetailRepository _orderDetailRepository;

        public OrderService(IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _orderRepository.GetByIdAsync(orderId);
        }

        public async Task<IEnumerable<Order>> GetOrdersByMemberIdAsync(int memberId)
        {
            return await _orderRepository.GetOrdersByMemberIdAsync(memberId);
        }


        public async Task CreateOrderAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            // Validate Order
            order.ValidateOrder();

            // Thêm Order vào DB trước
            order.OrderDate = DateTime.UtcNow;
            await _orderRepository.AddAsync(order);

            // Nếu đơn hàng có OrderDetails, thêm vào DB
            if (order.OrderDetails != null && order.OrderDetails.Any())
            {
                foreach (var orderDetail in order.OrderDetails)
                {
                    orderDetail.OrderId = order.OrderId; // Gán ID đơn hàng cho OrderDetail
                    await _orderDetailRepository.AddAsync(orderDetail);
                }
            }
        }

        public async Task UpdateOrderAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            // Validate Order trước khi cập nhật
            order.ValidateOrder();

            // Cập nhật thông tin Order
            await _orderRepository.UpdateAsync(order);

            // Xóa các OrderDetail cũ và thêm lại các OrderDetail mới (tránh lỗi sync dữ liệu)
            if (order.OrderDetails != null && order.OrderDetails.Any())
            {
                await _orderDetailRepository.DeleteByOrderIdAsync(order.OrderId);

                foreach (var orderDetail in order.OrderDetails)
                {
                    orderDetail.OrderId = order.OrderId;
                    await _orderDetailRepository.AddAsync(orderDetail);
                }
            }
        }

        public async Task DeleteOrderAsync(int orderId)
        {
            // Xóa tất cả OrderDetail trước để tránh lỗi ràng buộc khóa ngoại
            await _orderDetailRepository.DeleteByOrderIdAsync(orderId);

            // Xóa đơn hàng
            await _orderRepository.DeleteAsync(orderId);
        }
    }
}
