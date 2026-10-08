using Scharff.Test.Application.DTOs;
using Scharff.Test.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scharff.Test.Application.Interfaces
{
    public interface IClientNotifier
    {
        bool AppliesTo(string clientCode);
        Task SendNotificationAsync(Order order, TmsEventDto eventData);
    }
}
