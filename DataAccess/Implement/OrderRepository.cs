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
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate && o.Status == "Completed")
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

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Order.Include(o => o.Member).Include(o => o.OrderDetails).ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(int orderId)
        {
            return await _context.Order.Include(o => o.Member).Include(o => o.OrderDetails)
                                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<IEnumerable<Order>> GetOrdersByMemberIdAsync(int memberId)
        {
            return await _context.Order
                .Where(o => o.MemberId == memberId)
                .Include(o => o.OrderDetails)
                .ToListAsync();
        }

        public async Task AddAsync(Order order)
        {
            await _context.Order.AddAsync(order);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateAsync(Order order)
        {
            _context.Order.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int orderId)
        {
            var order = await GetByIdAsync(orderId);
            if (order != null)
            {
                _context.Order.Remove(order);
                await _context.SaveChangesAsync();
            }
        }
    }
}
