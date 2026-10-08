using Scharff.Test.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scharff.Test.Application.Interfaces
{
    public interface ICloudStorageService
    {
        Task<string> UploadAsync(string orderNumber, TmsEvidenceDto evidence);
    }
}
