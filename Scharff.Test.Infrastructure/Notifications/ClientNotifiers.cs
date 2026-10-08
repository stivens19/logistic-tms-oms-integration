using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Scharff.Test.Application.Configuration;
using Scharff.Test.Application.DTOs;
using Scharff.Test.Application.Interfaces;
using Scharff.Test.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Scharff.Test.Infrastructure.Notifications
{
    public class TiendasPeruanasNotifier : IClientNotifier
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<TiendasPeruanasNotifier> _logger;

        public TiendasPeruanasNotifier(
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            ILogger<TiendasPeruanasNotifier> logger)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
            _logger = logger;
        }

        public bool AppliesTo(string clientCode) => clientCode == "01021755";

        public async Task SendNotificationAsync(Order order, TmsEventDto eventData)
        {
            var payload = new
            {
                codigoPedido = order.OrderNumber,
                estadoOperativo = order.CurrentStatus,
                intentosVisita = order.VisitCount,
                evidencias = order.StoredEvidenceUrls,
                fechaActualizacion = DateTime.UtcNow
            };

            var targetUrl = _config["IntegrationConfig:TiendasPeruanasWebhookUrl"];

            if (!string.IsNullOrWhiteSpace(targetUrl))
            {
                try
                {
                    var client = _httpClientFactory.CreateClient();
                    var response = await client.PostAsJsonAsync(targetUrl, payload);
                    _logger.LogInformation("[Webhook Saliente Tiendas Peruanas] Enviado a {Url} con status {StatusCode}", targetUrl, response.StatusCode);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[Webhook Saliente Tiendas Peruanas] Falló el envío al webhook");
                }
            }
        }
    }
    public class DefaultClientNotifier : IClientNotifier
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<DefaultClientNotifier> _logger;

        public DefaultClientNotifier(
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            ILogger<DefaultClientNotifier> logger)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
            _logger = logger;
        }

        public bool AppliesTo(string clientCode) => clientCode == "DEFAULT";

        public async Task SendNotificationAsync(Order order, TmsEventDto eventData)
        {
            var payload = new
            {
                orderId = order.OrderNumber,
                clientCode = order.ClientCode,
                status = order.CurrentStatus,
                visits = order.VisitCount,
                updatedAt = DateTime.UtcNow
            };

            var targetUrl = _config["IntegrationConfig:DefaultWebhookUrl"];

            if (!string.IsNullOrWhiteSpace(targetUrl))
            {
                try
                {
                    var client = _httpClientFactory.CreateClient();
                    var response = await client.PostAsJsonAsync(targetUrl, payload);
                    _logger.LogInformation("[Webhook Saliente Default] Enviado a {Url} con status {StatusCode}", targetUrl, response.StatusCode);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[Webhook Saliente Default] Falló el envío al webhook");
                }
            }
        }
    }
}
