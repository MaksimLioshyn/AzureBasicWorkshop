using System.Text.Json;
using Azure.Identity;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Options;
using Contracts;
using WebAppGateway.Options;

namespace WebAppGateway.Messaging;

public sealed class EventHubPublisher : IEventHubPublisher
{
    private readonly MessagingOptions _options;
    private readonly IRetryPolicyExecutor _retryPolicy;

    public EventHubPublisher(IOptions<MessagingOptions> options, IRetryPolicyExecutor retryPolicy)
    {
        _options = options.Value;
        _retryPolicy = retryPolicy;
    }

    public async Task PublishAsync(MessageEnvelope envelope, CancellationToken cancellationToken)
    {
        await using var producer = new EventHubProducerClient(
            _options.EventHubFullyQualifiedNamespace,
            _options.EventHubName,
            new DefaultAzureCredential());

        await _retryPolicy.ExecuteAsync(
            async ct =>
            {
                using EventDataBatch batch = await producer.CreateBatchAsync(ct);
                var payload = JsonSerializer.SerializeToUtf8Bytes(envelope);
                if (!batch.TryAdd(new EventData(payload)))
                {
                    throw new InvalidOperationException("Could not add event to Event Hub batch.");
                }

                await producer.SendAsync(batch, ct);
            },
            _options.RetryAttempts,
            _options.RetryBaseDelayMs,
            cancellationToken);
    }
}