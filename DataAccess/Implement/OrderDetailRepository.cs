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
    public class OrderDetailRepository :IOrderDetailRepository
    {
        private readonly EStoreContext _context;

        public OrderDetailRepository(EStoreContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderDetail>> GetAllAsync()
        {
            return await _context.OrderDetail
                .Include(od => od.Order)
                .Include(od => od.Product)
                .ToListAsync();
        }

        public async Task<OrderDetail?> GetByIdAsync(int orderId, int productId)
        {
            return await _context.OrderDetail
                .Include(od => od.Order)
                .Include(od => od.Product)
                .FirstOrDefaultAsync(od => od.OrderId == orderId && od.ProductId == productId);
        }

        public async Task AddAsync(OrderDetail orderDetail)
        {
            await _context.OrderDetail.AddAsync(orderDetail);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(OrderDetail orderDetail)
        {
            _context.OrderDetail.Update(orderDetail);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int orderId, int productId)
        {
            var orderDetail = await GetByIdAsync(orderId, productId);
            if (orderDetail != null)
            {
                _context.OrderDetail.Remove(orderDetail);
                await _context.SaveChangesAsync();
            }
        }
        public async Task DeleteByOrderIdAsync(int orderId)
        {
            var orderDetails = _context.OrderDetail.Where(od => od.OrderId == orderId);
            _context.OrderDetail.RemoveRange(orderDetails);
            await _context.SaveChangesAsync();
        }
    }
}
