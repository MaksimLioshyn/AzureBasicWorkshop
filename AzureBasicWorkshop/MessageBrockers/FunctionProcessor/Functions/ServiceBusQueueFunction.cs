using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Contracts;

namespace FunctionProcessor.Functions;

public sealed class ServiceBusQueueFunction
{
    private readonly ILogger<ServiceBusQueueFunction> _logger;

    public ServiceBusQueueFunction(ILogger<ServiceBusQueueFunction> logger)
    {
        _logger = logger;
    }

    [Function("ProcessServiceBusQueueMessage")]
    public async Task Run(
        [ServiceBusTrigger("%ServiceBusQueueName%", Connection = "ServiceBusConnection")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions,
        CancellationToken cancellationToken)
    {
        try
        {
            var envelope = JsonSerializer.Deserialize<MessageEnvelope>(message.Body);
            _logger.LogInformation(
                "Service Bus Queue message processed. MessageId={MessageId}, EventType={EventType}, CorrelationId={CorrelationId}",
                message.MessageId,
                envelope?.EventType,
                envelope?.CorrelationId);

            await messageActions.CompleteMessageAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Service Bus Queue message processing failed. MessageId={MessageId}", message.MessageId);
            await messageActions.AbandonMessageAsync(message, cancellationToken: cancellationToken);
        }
    }
}