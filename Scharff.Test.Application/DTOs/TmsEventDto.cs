using Scharff.Test.Application.Common;
using System.Text.Json.Serialization;

namespace Scharff.Test.Application.DTOs;

public record TmsEvidenceDto(string Label, string FileType, string FileName, string Url);

public record TmsDetailsDto(
    string OrderNumber,
    string TrackingNumber,
    string ClientCode,
    string ClientName,
    string ReceivedBy,
    string Comments,
    List<TmsEvidenceDto>? Evidences
);

public record TmsEventDto(
    string ServiceType,
    string DispatchType,
    string Status,
    string SubStatus,
    string VehicleCode,
    string CourierName,
    TmsDetailsDto Details,
    DateTime EventDate
);