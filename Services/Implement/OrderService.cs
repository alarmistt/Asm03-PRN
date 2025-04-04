using BusinessObject.Entities;
using Core;
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
        private readonly IProductRepository _productRepository;

        public OrderService(IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository, IHubContext<ChatHub> hubContext, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _hubContext = hubContext;
            _productRepository = productRepository;
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
                await _hubContext.Clients.All.SendAsync("ReceiveMessage");
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



        public async Task<Order> CreateOrderAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));


            order.ValidateOrder();
            order.OrderDate = DateTime.Now;


            var mergedDetails = order.OrderDetails
                .GroupBy(od => od.ProductId)
                .Select(g => new OrderDetail
                {
                    ProductId = g.Key,
                    Quantity = g.Sum(od => od.Quantity),
                    UnitPrice = g.First().UnitPrice
                }).ToList();


            foreach (var orderDetail in mergedDetails)
            {
                var product = await _productRepository.GetByIdAsync(orderDetail.ProductId);
                if (product == null)
                {
                    throw new InvalidOperationException($"Product with ID {orderDetail.ProductId} not found.");
                }


                if (product.UnitsInStock < orderDetail.Quantity)
                {
                    throw new InvalidOperationException($"Insufficient stock for product {orderDetail.ProductId}. Available: {product.UnitsInStock}, Requested: {orderDetail.Quantity}.");
                }
                product.UnitsInStock -= orderDetail.Quantity;
                await _productRepository.UpdateAsync(product);
            }


            order.OrderDetails = mergedDetails;

            await _orderRepository.AddAsync(order);

            await _hubContext.Clients.All.SendAsync("ReceiveMessage");

            return order;
        }



        public async Task UpdateOrderAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));


            order.ValidateOrder();



            var existingDetails = order.OrderDetails;

            var newDetails = order.OrderDetails ?? new List<OrderDetail>();


            var detailsToDelete = existingDetails.Where(ed => !newDetails.Any(nd => nd.ProductId == ed.ProductId)).ToList();


            var detailsToAdd = newDetails.Where(nd => !existingDetails.Any(ed => ed.ProductId == nd.ProductId)).ToList();

            var detailsToUpdate = newDetails.Where(nd => existingDetails.Any(ed => ed.ProductId == nd.ProductId)).ToList();

            foreach (var detail in detailsToDelete)
            {
                await _orderDetailRepository.DeleteAsync(detail.OrderId);
            }


            foreach (var detail in detailsToAdd)
            {
                detail.OrderId = order.OrderId;
                await _orderDetailRepository.AddAsync(detail);
            }

            foreach (var detail in detailsToUpdate)
            {
                await _orderDetailRepository.UpdateAsync(detail);
            }


            await _orderRepository.UpdateAsync(order);

            await _hubContext.Clients.All.SendAsync("ReceiveMessage");
        }

        public async Task DeleteOrderAsync(int orderId)
        {

            await _orderDetailRepository.DeleteByOrderIdAsync(orderId);


            await _orderRepository.DeleteAsync(orderId);
            await _hubContext.Clients.All.SendAsync("ReceiveMessage");
        }

        public async Task UpdateOrderStatus(int orderId, string status)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
            {
                throw new InvalidOperationException($"Order with ID {orderId} not found.");
            }

            if (status == "Cancelled")
            {

                foreach (var orderDetail in order.OrderDetails)
                {
                    var product = await _productRepository.GetByIdAsync(orderDetail.ProductId);
                    if (product == null)
                    {
                        throw new InvalidOperationException($"Product with ID {orderDetail.ProductId} not found.");
                    }

                    product.UnitsInStock += orderDetail.Quantity;
                    await _productRepository.UpdateAsync(product);
                }
            }


            await _orderRepository.UpdateOrderStatus(orderId, status);

            await _hubContext.Clients.All.SendAsync("ReceiveMessage");
        }

        public async Task<PaginatedList<Order>> GetOrders(int pageNumber, int pageSize)
        {
            return await _orderRepository.GetOrders(pageNumber, pageSize);
        }

        public async Task<PaginatedList<Order>> GetOrdersByMemberId(int memberId, int pageNumber, int pageSize)
        {
            return await _orderRepository.GetOrdersByMemberId(memberId, pageNumber, pageSize);

        }
    }
}
