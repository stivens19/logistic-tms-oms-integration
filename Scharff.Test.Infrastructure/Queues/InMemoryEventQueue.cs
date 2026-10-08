using Scharff.Test.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace Scharff.Test.Infrastructure.Queues
{
    public class InMemoryEventQueue
    {
        private readonly Channel<TmsEventDto> _channel = Channel.CreateUnbounded<TmsEventDto>();

        public ValueTask EnqueueAsync(TmsEventDto evt) => _channel.Writer.WriteAsync(evt);
        public IAsyncEnumerable<TmsEventDto> ReadAllAsync(CancellationToken ct) => _channel.Reader.ReadAllAsync(ct);
    }
}
