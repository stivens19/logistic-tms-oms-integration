using Scharff.Test.Application.Interfaces;
using Scharff.Test.Domain.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Scharff.Test.Infrastructure.Persistence
{
    public class InMemoryOrderRepository : IOrderRepository
    {
        private readonly ConcurrentDictionary<string, Order> _orders = new();
        private readonly ConcurrentBag<OrderHistory> _history = new();

        public Task<Order?> GetByOrderNumberAsync(string orderNumber)
        {
            _orders.TryGetValue(orderNumber, out var order);
            return Task.FromResult(order);
        }

        public Task SaveAsync(Order order)
        {
            _orders[order.OrderNumber] = order;
            return Task.CompletedTask;
        }

        public Task AddHistoryAsync(OrderHistory history)
        {
            _history.Add(history);
            return Task.CompletedTask;
        }
        public Task<IEnumerable<OrderHistory>> GetHistoryByOrderAsync(string orderNumber)
        {
            var historyList = _history
                .Where(h => string.Equals(h.OrderNumber, orderNumber, StringComparison.OrdinalIgnoreCase))
                .OrderBy(h => h.EventDate)
                .ToList();

            return Task.FromResult<IEnumerable<OrderHistory>>(historyList);
        }
    }
}
