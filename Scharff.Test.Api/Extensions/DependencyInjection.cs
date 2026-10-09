using Scharff.Test.Application.Configuration;
using Scharff.Test.Application.Interfaces;
using Scharff.Test.Application.UseCases;
using Scharff.Test.Infrastructure.Notifications;
using Scharff.Test.Infrastructure.Persistence;
using Scharff.Test.Infrastructure.Queues;
using Scharff.Test.Infrastructure.Services;
using Scharff.Test.Infrastructure.Storage;
using Scharff.Test.Infrastructure.Workers;

namespace Scharff.Test.Api.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<IntegrationOptions>(configuration.GetSection(IntegrationOptions.SectionName));

            services.AddScoped<ProcessTmsEventUseCase>();

            services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
            services.AddSingleton<ICloudStorageService, CloudStorageMockService>();

            services.AddSingleton<IClientNotifier, TiendasPeruanasNotifier>();
            services.AddSingleton<IClientNotifier, DefaultClientNotifier>();

            services.AddSingleton<InMemoryEventQueue>();
            services.AddHostedService<EventProcessorWorker>();

            services.AddScoped<ITokenService, JwtTokenService>();

            return services;
        }
    }
}
