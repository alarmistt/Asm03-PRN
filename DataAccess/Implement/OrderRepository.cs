using BusinessObject.Entities;
using DataAccess.Base;
using DataAccess.Interface;
using Microsoft.EntityFrameworkCore;
using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Implement
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IDbContextFactory<EStoreContext> _contextFactory;

        public OrderRepository(IDbContextFactory<EStoreContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Order>> GetOrdersByDateRange(DateTime startDate, DateTime endDate)
        {
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                return await context.Order
                    .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                    .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate && o.Status == "Completed")
                    .ToListAsync();
            }
        }

        public async Task<List<Order>> GetPendingOrdersByDate(DateTime date)
        {
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                return await context.Order
                    .Where(o => o.OrderDate.Date == date.Date && o.Status == "Pending")
                    .ToListAsync();
            }
        }

        public async Task UpdateOrderStatus(int orderId, string newStatus)
        {
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                var order = await context.Order.FindAsync(orderId);
                if (order != null)
                {
                    order.Status = newStatus;
                    if (newStatus == "Completed")
                    {
                        order.ShippedDate = DateTime.Now;
                    }
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                return await context.Order.AsNoTracking()
                    .Include(o => o.Member)
                    .Include(o => o.OrderDetails)
                    .ToListAsync();
            }
        }

        public async Task<PaginatedList<Order>> GetOrders(int pageNumber, int pageSize)
        {
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                var query = context.Order.AsNoTracking()
                    .Include(o => o.Member)
                    .Include(o => o.OrderDetails)
                    .OrderByDescending(x => x.OrderId);
                return await PaginatedList<Order>.CreateAsync(query, pageNumber, pageSize);
            }
        }

        public async Task<PaginatedList<Order>> GetOrdersByMemberId(int memberId, int pageNumber, int pageSize)
        {
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                var query = context.Order.AsNoTracking()
                    .Where(o => o.MemberId == memberId)
                    .Include(o => o.Member)
                    .Include(o => o.OrderDetails)
                    .OrderByDescending(x => x.OrderId);
                return await PaginatedList<Order>.CreateAsync(query, pageNumber, pageSize);
            }
        }

        public async Task<Order?> GetByIdAsync(int orderId)
        {
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                return await context.Order
                    .Include(o => o.Member)
                    .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersByMemberIdAsync(int memberId)
        {
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                return await context.Order
                    .Where(o => o.MemberId == memberId)
                    .Include(o => o.OrderDetails)
                    .ToListAsync();
            }
        }

        public async Task AddAsync(Order order)
        {
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                await context.Order.AddAsync(order);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(Order order)
        {
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                context.Order.Update(order);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int orderId)
        {
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                var order = await context.Order.FindAsync(orderId);
                if (order != null)
                {
                    context.Order.Remove(order);
                    await context.SaveChangesAsync();
                }
            }
        }
    }

}
