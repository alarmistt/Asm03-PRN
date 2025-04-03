using BusinessObject.Entities;
using DataAccess.Base;
using DataAccess.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Implement
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly IDbContextFactory<EStoreContext> _contextFactory;

        public OrderDetailRepository(IDbContextFactory<EStoreContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<OrderDetail>> GetAllAsync()
        {
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                return await context.OrderDetail
                    .Include(od => od.Order)
                    .Include(od => od.Product)
                    .ToListAsync();
            }
        }

        public async Task<OrderDetail?> GetByIdAsync(int orderId)
        {
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                return await context.OrderDetail
                    .Include(od => od.Order)
                    .Include(od => od.Product)
                    .FirstOrDefaultAsync(od => od.OrderId == orderId);
            }
        }

        public async Task AddAsync(OrderDetail orderDetail)
        {
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                await context.OrderDetail.AddAsync(orderDetail);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(OrderDetail orderDetail)
        {
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                context.OrderDetail.Update(orderDetail);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int orderId)
        {
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                var orderDetail = await context.OrderDetail.FirstOrDefaultAsync(od => od.OrderId == orderId);
                if (orderDetail != null)
                {
                    context.OrderDetail.Remove(orderDetail);
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task DeleteByOrderIdAsync(int orderId)
        {
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                var orderDetails = await context.OrderDetail.Where(od => od.OrderId == orderId).ToListAsync();
                if (orderDetails.Any())
                {
                    context.OrderDetail.RemoveRange(orderDetails);
                    await context.SaveChangesAsync();
                }
            }
        }
    }

}
