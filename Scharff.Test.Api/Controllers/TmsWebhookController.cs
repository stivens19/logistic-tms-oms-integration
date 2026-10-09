using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Scharff.Test.Api.Extensions;
using Scharff.Test.Api.Filters;
using Scharff.Test.Application.DTOs;
using Scharff.Test.Domain.Constants;
using Scharff.Test.Infrastructure.Queues;

namespace Scharff.Test.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/webhooks")]
    public class TmsWebhookController : ControllerBase
    {
        private readonly InMemoryEventQueue _eventQueue;
        private readonly ILogger<TmsWebhookController> _logger;

        public TmsWebhookController(InMemoryEventQueue eventQueue, ILogger<TmsWebhookController> logger)
        {
            _eventQueue = eventQueue;
            _logger = logger;
        }

        [HttpPost("tms")]
        [EnableRateLimiting(RateLimiterExtensions.WebhookPolicyName)]
        [ValidateWebhookSignature]
        [RequestSizeLimit(2 * 1024 * 1024)] 
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ReceiveTmsEvent([FromBody] TmsEventDto payload)
        {
            if (payload?.Details == null || string.IsNullOrWhiteSpace(payload.Details.OrderNumber))
            {
                return BadRequest(new { message = "Payload inválido. El campo 'details.orderNumber' es requerido." });
            }
            if (!OrderStatus.IsValid(payload.Status))
            {
                return BadRequest(new
                {
                    message = $"El estado '{payload?.Status}' no es un estado válido del TMS.",
                    allowedStatuses = OrderStatus.All
                });
            }

            _logger.LogInformation("Evento recibido para el pedido {OrderNumber}. Encolando...", payload.Details.OrderNumber);

            await _eventQueue.EnqueueAsync(payload);

            return Accepted(new
            {
                message = "Evento recibido y encolado satisfactoriamente para su procesamiento asíncrono.",
                orderNumber = payload.Details.OrderNumber,
                status = payload.Status
            });
        }
    }
}
