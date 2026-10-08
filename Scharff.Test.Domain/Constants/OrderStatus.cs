using System;
using System.Collections.Generic;
using System.Text;

namespace Scharff.Test.Domain.Constants
{
    public static class OrderStatus
    {
        public const string Planning = "PLANNING";
        public const string Started = "STARTED";
        public const string AtPickupPoint = "AT PICKUP POINT";
        public const string Collected = "COLLECTED";
        public const string NotCollected = "NOT COLLECTED";
        public const string Delivered = "DELIVERED";
        public const string NotDelivered = "NOT DELIVERED";
        public const string ToBeReturn = "TO BE RETURN";
        public const string Returned = "RETURNED";
        public const string NotReturned = "NOT RETURNED";

        public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            Planning,
            Started,
            AtPickupPoint,
            Collected,
            NotCollected,
            Delivered,
            NotDelivered,
            ToBeReturn,
            Returned,
            NotReturned
        };

        public static readonly IReadOnlySet<string> TerminalStatuses = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            Delivered,
            Returned
        };

        public static readonly IReadOnlySet<string> VisitIncrementStatuses = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            Delivered,
            NotDelivered
        };

        public static readonly IReadOnlySet<string> EvidenceRequiredStatuses = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            Collected,
            NotCollected,
            Delivered,
            NotDelivered,
            Returned,
            NotReturned
        };

        public static bool IsValid(string? status) => !string.IsNullOrWhiteSpace(status) && All.Contains(status);
        public static bool IsTerminal(string? status) => !string.IsNullOrWhiteSpace(status) && TerminalStatuses.Contains(status);
        public static bool IncrementsVisit(string? status) => !string.IsNullOrWhiteSpace(status) && VisitIncrementStatuses.Contains(status);
        public static bool RequiresEvidence(string? status) => !string.IsNullOrWhiteSpace(status) && EvidenceRequiredStatuses.Contains(status);
    }
}
