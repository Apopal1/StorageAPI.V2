using StorageManagement.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StorageManagement.API.Repositories
{
    public interface IOutgoingOrderRepository
    {
        Task<IEnumerable<OutgoingOrder>> GetAllAsync();
        Task<OutgoingOrder> GetByIdAsync(int id);
        Task<OutgoingOrder> CreateAsync(OutgoingOrder order);
        Task UpdateAsync(OutgoingOrder order);
        Task DeleteAsync(int id);
        Task AddOrderItemAsync(OutgoingOrderItem item);
        Task<IEnumerable<OutgoingOrderItem>> GetOrderItemsAsync(int orderId);
    }
}