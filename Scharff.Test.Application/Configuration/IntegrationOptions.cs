using System;
using System.Collections.Generic;
using System.Text;

namespace Scharff.Test.Application.Configuration
{
    public class IntegrationOptions
    {
        public const string SectionName = "IntegrationConfig";

        public string StorageBaseUrl { get; set; } = string.Empty;
        public string TiendasPeruanasWebhookUrl { get; set; } = string.Empty;
        public int MaxDeliveryAttempts { get; set; } = 3;
        public int RetryMaxAttempts { get; set; } = 3;
        public int RetryBaseDelayMilliseconds { get; set; } = 200;
        public string TmsApiKey { get; set; } = string.Empty;
    }
}
