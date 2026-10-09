using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Scharff.Test.Api.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ValidateWebhookSignatureAttribute : Attribute, IAsyncActionFilter
    {
        private const string SignatureHeader = "X-Hub-Signature-256";
        private const string SecretConfigKey = "IntegrationConfig:TmsApiKey";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var httpContext = context.HttpContext;
            var configuration = httpContext.RequestServices.GetRequiredService<IConfiguration>();
            var secret = configuration[SecretConfigKey] ?? "local-dev-api-key-12345";

            if (!httpContext.Request.Headers.TryGetValue(SignatureHeader, out var signatureHeaderValue) ||
                string.IsNullOrWhiteSpace(signatureHeaderValue))
            {
                if (!httpContext.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment())
                {
                    context.Result = new UnauthorizedObjectResult(new { error = "Encabezado X-Hub-Signature-256 requerido." });
                    return;
                }

                await next();
                return;
            }

            httpContext.Request.Body.Position = 0;
            using var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, leaveOpen: true);
            var rawBody = await reader.ReadToEndAsync();

            httpContext.Request.Body.Position = 0;

            var cleanBody = rawBody.Replace("\r\n", "\n").Trim();

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(cleanBody));
            var expectedSignature = "sha256=" + Convert.ToHexString(hash).ToLowerInvariant();
            var receivedSignature = signatureHeaderValue.ToString().Trim();

            if (!CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(expectedSignature),
                    Encoding.UTF8.GetBytes(receivedSignature)))
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    error = "Firma criptográfica inválida (OWASP API2 - Integrity violation).",
                    esperado = expectedSignature,
                    recibido = receivedSignature,
                    longitudBodyBackend = cleanBody.Length
                });
                return;
            }

            await next();
        }
    }
}
