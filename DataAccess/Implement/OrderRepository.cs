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

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Set<Order>().Include(o => o.Member).Include(o => o.OrderDetails).ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(int orderId)
        {
            return await _context.Set<Order>().Include(o => o.Member).Include(o => o.OrderDetails)
                                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<bool> AddAsync(Order order)
        {
            var exist = await _context.Set<Order>().FindAsync(order.OrderId);
            if (exist != null)
            {
                throw new Exception("Order already exists");
            }
            await _context.Set<Order>().AddAsync(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(Order order)
        {
            var exist = await _context.Set<Order>().FindAsync(order.OrderId);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(order);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteAsync(int orderId)
        {
            var order = await _context.Set<Order>().FindAsync(orderId);
            if (order != null)
            {
                _context.Set<Order>().Remove(order);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
