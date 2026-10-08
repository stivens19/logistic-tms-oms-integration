using Scharff.Test.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scharff.Test.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetByOrderNumberAsync(string orderNumber);
        Task SaveAsync(Order order);
        Task AddHistoryAsync(OrderHistory history);
        Task<IEnumerable<OrderHistory>> GetHistoryByOrderAsync(string orderNumber);
    }
}
