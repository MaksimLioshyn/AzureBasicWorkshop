using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Contracts;

namespace FunctionProcessor.Functions;

public sealed class ServiceBusTopicFunction
{
    private readonly ILogger<ServiceBusTopicFunction> _logger;

    public ServiceBusTopicFunction(ILogger<ServiceBusTopicFunction> logger)
    {
        _logger = logger;
    }

    [Function("ProcessServiceBusTopicMessage")]
    public async Task Run(
        [ServiceBusTrigger(
            "%ServiceBusTopicName%",
            "%ServiceBusTopicSubscriptionName%",
            Connection = "ServiceBusConnection")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions,
        CancellationToken cancellationToken)
    {
        try
        {
            var envelope = JsonSerializer.Deserialize<MessageEnvelope>(message.Body);
            _logger.LogInformation(
                "Service Bus Topic message processed. MessageId={MessageId}, EventType={EventType}, CorrelationId={CorrelationId}",
                message.MessageId,
                envelope?.EventType,
                envelope?.CorrelationId);

            await messageActions.CompleteMessageAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Service Bus Topic message processing failed. MessageId={MessageId}", message.MessageId);
            await messageActions.AbandonMessageAsync(message, cancellationToken: cancellationToken);
        }
    }
}