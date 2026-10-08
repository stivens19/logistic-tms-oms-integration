using Microsoft.Extensions.Logging;
using Scharff.Test.Application.DTOs;
using Scharff.Test.Application.Interfaces;
using Scharff.Test.Domain.Constants;
using Scharff.Test.Domain.Entities;

namespace Scharff.Test.Application.UseCases;

public class ProcessTmsEventUseCase
{
    private readonly IOrderRepository _repository;
    private readonly ICloudStorageService _storageService;
    private readonly IEnumerable<IClientNotifier> _notifiers;
    private readonly ILogger<ProcessTmsEventUseCase> _logger;

    public ProcessTmsEventUseCase(
        IOrderRepository repository,
        ICloudStorageService storageService,
        IEnumerable<IClientNotifier> notifiers,
        ILogger<ProcessTmsEventUseCase> logger)
    {
        _repository = repository;
        _storageService = storageService;
        _notifiers = notifiers;
        _logger = logger;
    }

    public async Task ExecuteAsync(TmsEventDto eventDto)
    {
        var orderNumber = eventDto.Details.OrderNumber;
        var order = await _repository.GetByOrderNumberAsync(orderNumber);

        if (order == null)
        {
            order = new Order(orderNumber, eventDto.Details.ClientCode, eventDto.Status);
        }
        if (order.IsInTerminalState())
        {
            _logger.LogWarning("[Regla Ignorada] Orden {OrderNumber} ya se encuentra en estado terminal ({Status}).",
                order.OrderNumber, order.CurrentStatus);
            return;
        }
        order.ApplyEventStatus(eventDto.Status);

        if (OrderStatus.RequiresEvidence(eventDto.Status) && eventDto.Details.Evidences?.Count > 0)
        {
            foreach (var evidence in eventDto.Details.Evidences)
            {
                var storedUrl = await _storageService.UploadAsync(order.OrderNumber, evidence);
                order.AddEvidenceUrl(storedUrl);
            }
        }

        await _repository.SaveAsync(order);
        await _repository.AddHistoryAsync(new OrderHistory
        {
            OrderNumber = order.OrderNumber,
            Status = order.CurrentStatus,
            SubStatus = eventDto.SubStatus ?? string.Empty,
            CourierName = eventDto.CourierName ?? string.Empty,
            EventDate = eventDto.EventDate
        });

        var notifier = _notifiers.FirstOrDefault(n => n.AppliesTo(order.ClientCode))
                       ?? _notifiers.First(n => n.AppliesTo("DEFAULT"));

        await notifier.SendNotificationAsync(order, eventDto);
    }
}