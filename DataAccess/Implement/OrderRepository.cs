using BusinessObject.Base;
using BusinessObject.Entities;
using DataAccess.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Implement
{
    public class OrderRepository : IOrderRepository
    {
        private readonly EStoreContext _context;

        public OrderRepository(EStoreContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetOrdersByDateRange(DateTime startDate, DateTime endDate)
        {
            return await _context.Order
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate && o.Status == "Complete")
                .ToListAsync();
        }

        public async Task<List<Order>> GetPendingOrdersByDate(DateTime date)
        {
            return await _context.Order
                .Where(o => o.OrderDate.Date == date.Date
                    && o.Status == "Pending")
                .ToListAsync();
        }

        public async Task UpdateOrderStatus(int orderId, string newStatus)
        {
            var order = await _context.Order.FindAsync(orderId);
            if (order != null)
            {
                order.Status = newStatus;
                await _context.SaveChangesAsync();
            }
        }
    }
}
