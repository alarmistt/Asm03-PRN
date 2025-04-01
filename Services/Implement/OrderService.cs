using DataAccess.Interface;
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

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
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
    }
}
