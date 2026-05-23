using Azure;
using Azure.Identity;
using Azure.Messaging;
using Azure.Messaging.EventGrid;
using Microsoft.Extensions.Options;
using Contracts;
using WebAppGateway.Options;

namespace WebAppGateway.Messaging;

public sealed class EventGridPublisher : IEventGridPublisher
{
    private readonly MessagingOptions _options;
    private readonly IRetryPolicyExecutor _retryPolicy;

    public EventGridPublisher(IOptions<MessagingOptions> options, IRetryPolicyExecutor retryPolicy)
    {
        _options = options.Value;
        _retryPolicy = retryPolicy;
    }

    public async Task PublishAsync(MessageEnvelope envelope, CancellationToken cancellationToken)
    {
        var endpoint = new Uri(_options.EventGridTopicEndpoint);
        var client = new EventGridPublisherClient(endpoint, new DefaultAzureCredential());

        await _retryPolicy.ExecuteAsync(
            ct => client.SendEventAsync(BuildCloudEvent(envelope), ct),
            _options.RetryAttempts,
            _options.RetryBaseDelayMs,
            cancellationToken);
    }

    private static CloudEvent BuildCloudEvent(MessageEnvelope envelope)
    {
        return new CloudEvent(
            source: $"/webapps/{envelope.Source}",
            type: envelope.EventType,
            jsonSerializableData: envelope)
        {
            Subject = envelope.CorrelationId,
            Time = envelope.CreatedAt,
            DataContentType = "application/json"
        };
    }
}