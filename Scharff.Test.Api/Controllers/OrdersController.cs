using Microsoft.AspNetCore.Mvc;
using Scharff.Test.Application.Interfaces;

namespace Scharff.Test.Api.Controllers
{
    [ApiController]
    [Route("api/v1/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrdersController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet("{orderNumber}/history")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrderHistory(string orderNumber)
        {
            var history = await _orderRepository.GetHistoryByOrderAsync(orderNumber);

            if (!history.Any())
            {
                return NotFound(new { message = $"No se encontró historial para el pedido '{orderNumber}'." });
            }

            return Ok(history);
        }

        [HttpGet("{orderNumber}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrder(string orderNumber)
        {
            var order = await _orderRepository.GetByOrderNumberAsync(orderNumber);

            if (order == null)
            {
                return NotFound(new { message = $"El pedido '{orderNumber}' no existe." });
            }

            return Ok(order);
        }
    }
}
