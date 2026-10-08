using Microsoft.Extensions.Options;
using Scharff.Test.Application.Configuration;
using Scharff.Test.Application.DTOs;
using Scharff.Test.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scharff.Test.Infrastructure.Storage
{
    public class CloudStorageMockService : ICloudStorageService
    {
        private readonly IntegrationOptions _options;

        public CloudStorageMockService(IOptions<IntegrationOptions> options)
        {
            _options = options.Value;
        }
        public async Task<string> UploadAsync(string orderNumber, TmsEvidenceDto evidence)
        {
            await Task.Delay(20);
            return $"{_options.StorageBaseUrl}/{orderNumber}/{evidence.FileName}";
        }
    }
}
