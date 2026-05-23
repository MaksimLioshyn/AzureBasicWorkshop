using Microsoft.AspNetCore.Mvc;
using Contracts;
using WebAppGateway.Messaging;

namespace WebAppGateway.Controllers;

[ApiController]
[Route("api/messaging")]
public sealed class MessagingController : ControllerBase
{
    private readonly IServiceBusPublisher _serviceBusPublisher;
    private readonly IEventHubPublisher _eventHubPublisher;
    private readonly IEventGridPublisher _eventGridPublisher;

    public MessagingController(
        IServiceBusPublisher serviceBusPublisher,
        IEventHubPublisher eventHubPublisher,
        IEventGridPublisher eventGridPublisher)
    {
        _serviceBusPublisher = serviceBusPublisher;
        _eventHubPublisher = eventHubPublisher;
        _eventGridPublisher = eventGridPublisher;
    }

    [HttpPost("servicebus/queue")]
    public async Task<IActionResult> SendToServiceBusQueue([FromBody] SendMessageRequest request, CancellationToken cancellationToken)
    {
        MessageEnvelope envelope = BuildEnvelope(request, "servicebus.queue");
        await _serviceBusPublisher.SendToQueueAsync(envelope, cancellationToken);
        return Accepted(new { message = "Sent to Service Bus queue.", envelope.CorrelationId });
    }

    [HttpPost("servicebus/topic")]
    public async Task<IActionResult> SendToServiceBusTopic([FromBody] SendMessageRequest request, CancellationToken cancellationToken)
    {
        MessageEnvelope envelope = BuildEnvelope(request, "servicebus.topic");
        await _serviceBusPublisher.SendToTopicAsync(envelope, cancellationToken);
        return Accepted(new { message = "Sent to Service Bus topic.", envelope.CorrelationId });
    }

    [HttpPost("eventhub")]
    public async Task<IActionResult> PublishToEventHub([FromBody] SendMessageRequest request, CancellationToken cancellationToken)
    {
        MessageEnvelope envelope = BuildEnvelope(request, "eventhub.telemetry");
        await _eventHubPublisher.PublishAsync(envelope, cancellationToken);
        return Accepted(new { message = "Published to Event Hub.", envelope.CorrelationId });
    }

    [HttpPost("eventgrid")]
    public async Task<IActionResult> PublishToEventGrid([FromBody] SendMessageRequest request, CancellationToken cancellationToken)
    {
        MessageEnvelope envelope = BuildEnvelope(request, "eventgrid.domainEvent");
        await _eventGridPublisher.PublishAsync(envelope, cancellationToken);
        return Accepted(new { message = "Published to Event Grid topic.", envelope.CorrelationId });
    }

    private static MessageEnvelope BuildEnvelope(SendMessageRequest request, string defaultEventType)
    {
        var correlationId = string.IsNullOrWhiteSpace(request.CorrelationId)
            ? Guid.NewGuid().ToString("N")
            : request.CorrelationId.Trim();

        return new MessageEnvelope(
            Source: "WebAppGateway",
            EventType: string.IsNullOrWhiteSpace(request.EventType) ? defaultEventType : request.EventType.Trim(),
            Payload: request.Payload,
            CreatedAt: DateTimeOffset.UtcNow,
            CorrelationId: correlationId);
    }
}