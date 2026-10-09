using System;
using System.Collections.Generic;
using System.Text;

namespace Scharff.Test.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(string clientId, string role);
    }
}
