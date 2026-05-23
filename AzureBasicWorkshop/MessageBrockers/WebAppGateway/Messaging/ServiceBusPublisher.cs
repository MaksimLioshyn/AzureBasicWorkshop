using System.Text.Json;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using Contracts;
using WebAppGateway.Options;

namespace WebAppGateway.Messaging;

public sealed class ServiceBusPublisher : IServiceBusPublisher
{
    private readonly MessagingOptions _options;
    private readonly IRetryPolicyExecutor _retryPolicy;

    public ServiceBusPublisher(IOptions<MessagingOptions> options, IRetryPolicyExecutor retryPolicy)
    {
        _options = options.Value;
        _retryPolicy = retryPolicy;
    }

    public async Task SendToQueueAsync(MessageEnvelope envelope, CancellationToken cancellationToken)
    {
        await using var client = new ServiceBusClient(
            _options.ServiceBusFullyQualifiedNamespace,
            new DefaultAzureCredential());

        ServiceBusSender sender = client.CreateSender(_options.ServiceBusQueueName);

        await _retryPolicy.ExecuteAsync(
            async ct =>
            {
                ServiceBusMessage message = BuildMessage(envelope);
                await sender.SendMessageAsync(message, ct);
            },
            _options.RetryAttempts,
            _options.RetryBaseDelayMs,
            cancellationToken);
    }

    public async Task SendToTopicAsync(MessageEnvelope envelope, CancellationToken cancellationToken)
    {
        await using var client = new ServiceBusClient(
            _options.ServiceBusFullyQualifiedNamespace,
            new DefaultAzureCredential());

        ServiceBusSender sender = client.CreateSender(_options.ServiceBusTopicName);

        await _retryPolicy.ExecuteAsync(
            async ct =>
            {
                ServiceBusMessage message = BuildMessage(envelope);
                await sender.SendMessageAsync(message, ct);
            },
            _options.RetryAttempts,
            _options.RetryBaseDelayMs,
            cancellationToken);
    }

    private static ServiceBusMessage BuildMessage(MessageEnvelope envelope)
    {
        var payloadJson = JsonSerializer.Serialize(envelope);

        return new ServiceBusMessage(payloadJson)
        {
            ContentType = "application/json",
            Subject = envelope.EventType,
            MessageId = envelope.CorrelationId
        };
    }
}