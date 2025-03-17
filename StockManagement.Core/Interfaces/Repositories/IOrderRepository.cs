using StockManagement.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockManagement.Core.Interfaces.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IReadOnlyList<Order>> GetOrdersWithItemsAsync();
        Task<Order> GetOrderWithItemsAsync(int id);
        Task<IEnumerable<OrderItem>> GetOrderItemsByProductIdAsync(int productId);
        void DeleteOrderItem(OrderItem orderItem);
    }
}
