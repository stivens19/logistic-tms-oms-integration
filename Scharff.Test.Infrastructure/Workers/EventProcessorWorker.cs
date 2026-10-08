using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using Scharff.Test.Application.Configuration;
using Scharff.Test.Application.UseCases;
using Scharff.Test.Infrastructure.Queues;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scharff.Test.Infrastructure.Workers
{
    public class EventProcessorWorker : BackgroundService
    {
        private readonly InMemoryEventQueue _queue;
        private readonly IServiceProvider _serviceProvider;
        private readonly AsyncRetryPolicy _retryPolicy;

        public EventProcessorWorker(
                InMemoryEventQueue queue,
                IServiceProvider serviceProvider,
                IOptions<IntegrationOptions> options)
        {
            _queue = queue;
            _serviceProvider = serviceProvider;

            var config = options.Value;

            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    config.RetryMaxAttempts,
                    attempt => TimeSpan.FromMilliseconds(config.RetryBaseDelayMilliseconds * attempt),
                    (exception, timeSpan, attempt, context) =>
                    {
                        Console.WriteLine($"[Reintento {attempt}/{config.RetryMaxAttempts}] Fallo detectado. Esperando {timeSpan.TotalMilliseconds}ms...");
                    });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var evt in _queue.ReadAllAsync(stoppingToken))
            {
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    using var scope = _serviceProvider.CreateScope();
                    var useCase = scope.ServiceProvider.GetRequiredService<ProcessTmsEventUseCase>();
                    await useCase.ExecuteAsync(evt);
                });
            }
        }
    }
}
