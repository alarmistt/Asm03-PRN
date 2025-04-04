using BusinessObject.Entities;
using Core;
using Services.Models.SaleReport;

namespace Services.Interface
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<Order> CreateOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
        Task DeleteOrderAsync(int orderId);
        Task<IEnumerable<Order>> GetOrdersByMemberIdAsync(int memberId);
        Task<List<SalesReportDto>> GetSalesReport(DateTime startDate, DateTime endDate);
        Task CheckAndUpdatePendingOrders();
        Task UpdateOrderStatus(int orderId, string status);
        Task<PaginatedList<Order>> GetOrders(int pageNumber, int pageSize);
        Task<PaginatedList<Order>> GetOrdersByMemberId(int memberId, int pageNumber, int pageSize);
    }
}
