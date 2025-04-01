using BusinessObject.Entities;
using DataAccess.Interface;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services.Interface;
using Services.Models.SaleReport;
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
        private readonly IHubContext<ChatHub> _hubContext;

        public OrderService(IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository, IHubContext<ChatHub> hubContext)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _hubContext = hubContext;
        }

        public async Task<List<SalesReportDto>> GetSalesReport(DateTime startDate, DateTime endDate)
        {
            var orders = await _orderRepository.GetOrdersByDateRange(startDate, endDate);

            var salesReport = orders
                .SelectMany(o => o.OrderDetails)
                .GroupBy(od => new { od.ProductId, od.Product.ProductName })
                .Select(g => new SalesReportDto
                {
                    ProductName = g.Key.ProductName,
                    TotalQuantity = g.Sum(od => od.Quantity),
                    TotalSales = g.Sum(od => od.UnitPrice * od.Quantity * (1 - (decimal)od.Discount))
                })
                .OrderByDescending(r => r.TotalSales)
                .ToList();

            return salesReport;
        }

        public async Task CheckAndUpdatePendingOrders()
        {
            try
            {
                DateTime currentDate = DateTime.Now;

                var pendingOrders = await _orderRepository
                    .GetPendingOrdersByDate(currentDate);

                foreach (var order in pendingOrders)
                {
                    TimeSpan timeDifference = currentDate - order.OrderDate;

                    if (timeDifference.TotalMinutes > 30)
                    {
                        await _orderRepository.UpdateOrderStatus(
                            order.OrderId,
                            "Cancelled"
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating order status: " + ex.Message);
            }
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


        //public async Task CreateOrderAsync(Order order)
        //{
        //    if (order == null)
        //        throw new ArgumentNullException(nameof(order));

        //    // Validate Order
        //    order.ValidateOrder();

        //    // Thêm Order vào DB trước
        //    order.OrderDate = DateTime.UtcNow;
        //    await _orderRepository.AddAsync(order);

        //    // Nếu đơn hàng có OrderDetails, thêm vào DB
        //    if (order.OrderDetails != null && order.OrderDetails.Any())
        //    {
        //        foreach (var orderDetail in order.OrderDetails)
        //        {
        //            orderDetail.OrderId = order.OrderId; // Gán ID đơn hàng cho OrderDetail
        //            await _orderDetailRepository.AddAsync(orderDetail);
        //        }
        //    }
        //}
        //public async Task CreateOrderAsync(Order order)
        //{
        //    if (order == null)
        //        throw new ArgumentNullException(nameof(order));

        //    // Validate Order
        //    order.ValidateOrder();

        //    // Gán ngày đặt hàng
        //    order.OrderDate = DateTime.UtcNow;

        //    // Thêm Order vào DB (SaveChanges ở đây sẽ tạo OrderId)
        //    await _orderRepository.AddAsync(order);

        //    // Nếu đơn hàng có OrderDetails, xử lý thêm vào DB
        //    if (order.OrderDetails != null && order.OrderDetails.Any())
        //    {
        //        // Gộp các OrderDetail trùng nhau dựa vào ProductId
        //        var mergedDetails = order.OrderDetails
        //            .GroupBy(od => od.ProductId)
        //            .Select(g => new OrderDetail
        //            {
        //                OrderId = order.OrderId,
        //                ProductId = g.Key,
        //                Quantity = g.Sum(od => od.Quantity),
        //                // Tính lại đơn giá nếu cần. Ví dụ: lấy đơn giá của sản phẩm đầu tiên
        //                UnitPrice = g.First().UnitPrice
        //            }).ToList();

        //        foreach (var orderDetail in mergedDetails)
        //        {
        //            await _orderDetailRepository.AddAsync(orderDetail);
        //        }
        //    }
        //}
        public async Task<Order> CreateOrderAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            // Validate Order
            order.ValidateOrder();
            order.OrderDate = DateTime.Now;

            // Gộp các OrderDetail trùng nhau dựa vào ProductId
            var mergedDetails = order.OrderDetails
                .GroupBy(od => od.ProductId)
                .Select(g => new OrderDetail
                {
                    ProductId = g.Key,
                    Quantity = g.Sum(od => od.Quantity),
                    // Lấy đơn giá của sản phẩm đầu tiên hoặc tính toán lại theo nghiệp vụ
                    UnitPrice = g.First().UnitPrice
                }).ToList();

            // Gán các OrderDetail đã gộp vào thuộc tính điều hướng của Order
            order.OrderDetails = mergedDetails;

            // Thêm Order (với các OrderDetail theo cascade)
            await _orderRepository.AddAsync(order);

            await _hubContext.Clients.All.SendAsync("ReceiveMessage");
            return order;
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

        public async Task UpdateOrderStatus(int orderId, string status)
        {
            await _orderRepository.UpdateOrderStatus(orderId, status);
        }
    }
}
