using Scharff.Test.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scharff.Test.UnitTests.Domain
{
    public class OrderTests
    {
        [Fact]
        public void ApplyEventStatus_WhenDeliveredOrNotDelivered_ShouldIncrementVisitCount()
        {
            var order = new Order("ORD-01", "CLI-01");

            order.ApplyEventStatus("NOT DELIVERED");

            Assert.Equal(1, order.VisitCount);
            Assert.Equal("NOT DELIVERED", order.CurrentStatus);
        }

        [Fact]
        public void ApplyEventStatus_WhenVisitsReachThreeAndNotDelivered_ShouldChangeStatusToToBeReturn()
        {
            var order = new Order("ORD-01", "CLI-01");

            order.ApplyEventStatus("NOT DELIVERED");
            order.ApplyEventStatus("NOT DELIVERED");
            order.ApplyEventStatus("NOT DELIVERED");

            Assert.Equal(3, order.VisitCount);
            Assert.Equal("TO BE RETURN", order.CurrentStatus);
        }

        [Fact]
        public void ApplyEventStatus_WhenAlreadyInTerminalStatus_ShouldThrowInvalidOperationException()
        {
            var order = new Order("ORD-01", "CLI-01");
            order.ApplyEventStatus("DELIVERED");

            Assert.Throws<InvalidOperationException>(() =>
            {
                order.ApplyEventStatus("STARTED");
            });
        }
    }
}
