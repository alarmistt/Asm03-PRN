using BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Interface
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order> GetByIdAsync(int orderId);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(int orderId);

        Task<IEnumerable<Order>> GetOrdersByMemberIdAsync(int memberId);
    }
}
