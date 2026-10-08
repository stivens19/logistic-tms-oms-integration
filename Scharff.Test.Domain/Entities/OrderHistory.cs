using System;
using System.Collections.Generic;
using System.Text;

namespace Scharff.Test.Domain.Entities
{
    public class OrderHistory
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string OrderNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string SubStatus { get; set; } = string.Empty;
        public string CourierName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    }
}
