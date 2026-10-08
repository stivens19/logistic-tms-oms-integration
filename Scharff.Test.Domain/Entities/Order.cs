using System;
using System.Collections.Generic;
using Scharff.Test.Domain.Constants;

namespace Scharff.Test.Domain.Entities;

public class Order
{
    public string OrderNumber { get; private set; }
    public string ClientCode { get; private set; }
    public string CurrentStatus { get; private set; }
    public int VisitCount { get; private set; }
    public List<string> StoredEvidenceUrls { get; private set; } = new();
    public DateTime LastUpdatedAt { get; private set; }

    public Order(string orderNumber, string clientCode, string initialStatus = OrderStatus.Planning)
    {
        OrderNumber = orderNumber;
        ClientCode = clientCode;
        CurrentStatus = initialStatus;
        VisitCount = 0;
        LastUpdatedAt = DateTime.UtcNow;
    }

    public bool IsInTerminalState() => OrderStatus.IsTerminal(CurrentStatus);

    public void ApplyEventStatus(string newStatus)
    {
        if (IsInTerminalState())
        {
            throw new InvalidOperationException(
                $"No se puede actualizar el pedido {OrderNumber}. Ya se encuentra en estado terminal: {CurrentStatus}");
        }

        if (OrderStatus.IncrementsVisit(newStatus))
        {
            VisitCount++;
        }
        if (VisitCount >= 3 && !string.Equals(newStatus, OrderStatus.Delivered, StringComparison.OrdinalIgnoreCase))
        {
            CurrentStatus = OrderStatus.ToBeReturn;
        }
        else
        {
            CurrentStatus = newStatus;
        }

        LastUpdatedAt = DateTime.UtcNow;
    }

    public void AddEvidenceUrl(string url)
    {
        if (!string.IsNullOrWhiteSpace(url))
        {
            StoredEvidenceUrls.Add(url);
        }
    }
}