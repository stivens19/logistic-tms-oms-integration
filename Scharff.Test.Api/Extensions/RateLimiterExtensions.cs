using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace Scharff.Test.Api.Extensions
{
    public static class RateLimiterExtensions
    {
        public const string WebhookPolicyName = "WebhookPolicy";

        public static IServiceCollection AddRateLimiterConfig(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddFixedWindowLimiter(WebhookPolicyName, opt =>
                {
                    opt.PermitLimit = 30; 
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 0;
                });
            });

            return services;
        }
    }
}
